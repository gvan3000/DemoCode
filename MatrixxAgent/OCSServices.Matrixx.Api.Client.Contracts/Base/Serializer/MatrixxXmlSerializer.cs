using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Base
{
    public class MatrixxXmlSerializer
    {
        private Dictionary<string, Type> _matrixxObjectTypes = null;

        private Dictionary<string, Type> MatrixxObjectTypes
        {
            get
            {
                if (_matrixxObjectTypes == null)
                    _matrixxObjectTypes = LoadMatrixxObjectTypes();

                return _matrixxObjectTypes;
            }
        }

        private Dictionary<string, Type> LoadMatrixxObjectTypes()
        {
            Dictionary<string, Type> result = null;

            var matrixxAssembly = Assembly.GetAssembly(typeof(MatrixxObject));
            if (matrixxAssembly == null)
                throw new Exception($"Assembly not found for {nameof(MatrixxObject)}");

            var matrixxTypes = matrixxAssembly.GetTypes().Where(t => t.BaseType == typeof(MatrixxObject) || t.GetCustomAttribute(typeof(MatrixxContractAttribute)) != null);
            if (matrixxTypes == null || matrixxTypes.Count() < 1)
                throw new Exception($"Types are missing for {nameof(MatrixxObject)} in assembly {matrixxAssembly.FullName}");

            MatrixxContractAttribute attribute = null;
            result = new Dictionary<string, Type>();
            foreach (var matrixxType in matrixxTypes)
            {
                attribute = matrixxType.GetCustomAttribute(typeof(MatrixxContractAttribute)) as MatrixxContractAttribute;
                string key = attribute != null ? attribute.Name : matrixxType.Name;
                if (!result.ContainsKey(key))
                    result.Add(key, matrixxType);
            }

            return result;
        }

        private MatrixxPropertyInfo[] GetPropertyInfoList<T>(Type type, T rootObject) where T : new()
        {
            return (from prop in type.GetProperties()
                    from att in prop.GetCustomAttributes(typeof(MatrixxContractMemberAttribute), true)
                    where prop.GetValue(rootObject) != null
                    select new MatrixxPropertyInfo
                    {
                        MatrixxName = ((MatrixxContractMemberAttribute)att).Name,
                        PropertyName = prop.Name,
                        Value = prop.GetValue(rootObject),
                        Type = prop.PropertyType
                    }).ToArray();
        }

        public string Serialize<T>(T rootObject) where T : new()
        {
            if (rootObject == null)
                return string.Empty;

            XDocument document = new XDocument();
            document.Add(SerializeObject(rootObject));

            var result = document.ToString();

            result = result.Replace("&gt;", ">");
            result = result.Replace("&lt;", "<");

            return result;
        }

        private XElement SerializeObject<T>(T rootObject) where T : new()
        {
            var type = typeof(T);

            return SerializeObject(rootObject, type);
        }

        private XElement SerializeObject(object rootObject, Type type, string alternativeName = null)
        {
            var att = (MatrixxContractAttribute)type.GetCustomAttributes(typeof(MatrixxContractAttribute), true).FirstOrDefault();
            XElement root = null;
            if (att == null)
            {
                if (alternativeName == null)
                {
                    throw new Exception($"Cannot serialize {type.Name}: Invalid naming in contract.");
                }
                root = new XElement(alternativeName);
            }
            else
                root = new XElement(att.Name);

            var properties = GetPropertyInfoList(type, rootObject);
            if (properties != null && properties.Count() > 0)
            {
                foreach (var property in properties)
                {
                    SerializeProperty(property, root);
                }
            }
            else
                root.Value = rootObject?.ToString();

            return root;

        }

        private void SerializeProperty(MatrixxPropertyInfo property, XElement root)
        {
            if (property.Type.IsGenericType && property.Type.GetGenericTypeDefinition() == typeof(List<>))
            {
                //var listElement = new XElement(property.MatrixxName);
                //root.Add(listElement);
                foreach (var listItem in (IList)property.Value)
                {
                    root.Add(SerializeObject(listItem, listItem.GetType(), property.MatrixxName));
                    //listElement.Add(SerializeObject(listItem, listItem.GetType(), property.MatrixxName));
                }
            }
            else if (property.Type.IsGenericType && property.Type == typeof(Dictionary<string, string>))
            {
                foreach (var dictItem in property.Value as Dictionary<string, string>)
                {
                    root.Add(new XElement(dictItem.Key, dictItem.Value));
                }
            }
            else if (property.Type.BaseType == typeof(MatrixxObject))
            {
                root.Add(SerializeObject(property.Value, property.Type, property.MatrixxName));
            }
            else
                root.Add(new XElement(property.MatrixxName, property.Value));
        }

        public T Deserialize<T>(string xml)
        {
            Type type = typeof(T);

            return (T)Deserialize(xml, type);
        }

        private object Deserialize(string xml, Type type)
        {
            var instance = Activator.CreateInstance(type);

            List<MatrixxPropertyInfo> properties = (from prop in type.GetProperties()
                                                    where prop.GetCustomAttribute(typeof(MatrixxContractMemberAttribute), true) != null
                                                    select new MatrixxPropertyInfo
                                                    {
                                                        MatrixxName = GetMatrixxName(prop),
                                                        PropertyName = prop.Name,
                                                        Type = prop.PropertyType
                                                    }).ToList();

            object objectValue;
            IEnumerable<XElement> elements = null;
            XDocument document = XDocument.Parse(xml);
            foreach (var matrixxPropertyInfo in properties)
            {
                objectValue = null;
                var objectProperty = type.GetProperty(matrixxPropertyInfo.PropertyName);
                if (objectProperty != null)
                {
                    if (objectProperty.PropertyType.IsGenericType && objectProperty.PropertyType == typeof(List<MatrixxObject>))
                    {
                        Type listItemType = null;
                        MatrixxObject objectToSet = null;
                        objectValue = new List<MatrixxObject>();

                        IEnumerable<XElement> descendantElements = document.Root.Elements();
                        foreach (var childElement in descendantElements)
                        {
                            if (MatrixxObjectTypes.TryGetValue(childElement.Name.LocalName, out listItemType))
                            {
                                objectToSet = Deserialize(childElement.ToString(), listItemType) as MatrixxObject;
                                if (objectToSet != null)
                                    ((List<MatrixxObject>)objectValue).Add(objectToSet);
                            }
                        }
                    }
                    else
                    {
                        elements = document.Root.Elements(matrixxPropertyInfo.MatrixxName);
                        if (elements == null || elements.Count() < 1)
                            continue;

                        if (objectProperty.PropertyType.IsGenericType && objectProperty.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            object objectToSet = null;
                            objectValue = Activator.CreateInstance(objectProperty.PropertyType);
                            var genericType = objectProperty.PropertyType.GenericTypeArguments[0];

                            foreach (var childElements in elements)
                            {
                                if (genericType.BaseType == typeof(MatrixxObject) || genericType == typeof(MatrixxObject))
                                    objectToSet = Deserialize(childElements.ToString(), genericType);
                                else
                                    objectToSet = GetObjectToAdd(genericType, childElements.Value);

                                objectProperty.PropertyType.GetMethod("Add").Invoke(objectValue, new[] { objectToSet });
                            }
                        }
                        else
                        {
                            XElement singleElement = elements.FirstOrDefault();
                            if (matrixxPropertyInfo.Type.BaseType == typeof(MatrixxObject) || matrixxPropertyInfo.Type == typeof(MatrixxObject))
                                objectValue = Deserialize(singleElement.ToString(), matrixxPropertyInfo.Type);
                            else
                                objectValue = GetObjectToAdd(objectProperty.PropertyType, singleElement.Value);
                        }
                    }

                    objectProperty.SetValue(instance, objectValue);
                }
            }

            return instance;
        }

        private string GetMatrixxName(PropertyInfo targetProperty)
        {
            string retVal = string.Empty;

            var attr = targetProperty.GetCustomAttribute(typeof(MatrixxContractMemberAttribute)) as MatrixxContractMemberAttribute;
            if (attr != null && !string.IsNullOrEmpty(attr.Name))
                retVal = attr.Name;
            else
                retVal = targetProperty.Name;

            return retVal;
        }

        private void CreateList(PropertyInfo objectProperty, MatrixxPropertyInfo propertyInfo)
        {
            var genericType = propertyInfo.Type.GenericTypeArguments[0];

            //var objectToAdd =
            //    genericType.IsAbstract
            //    ? GetImplementationToAdd(reader, genericType)
            //    : GetObjectToAdd(genericType, reader, propertyInfo.MatrixxName);

            //objectProperty.PropertyType.GetMethod("Add").Invoke(propertyInfo.Value, new[] { objectToAdd });
        }

        private object GetImplementationToAdd(XmlReader reader, Type genericType)
        {
            reader.Read();
            var currentObjectName = reader.Name;
            var typeToAdd =
                (from type in genericType.Assembly.GetTypes()
                 from att in type.GetCustomAttributes(typeof(MatrixxContractAttribute))
                 where ((MatrixxContractAttribute)att).Name == currentObjectName
                 select type).Single();

            return null; //GetObjectToAdd(typeToAdd, reader, currentObjectName);
        }
        private object GetObjectToAdd(Type type, string value)
        {
            object objectToAdd = null;
            if (type.BaseType == typeof(MatrixxObject) || type == typeof(MatrixxObject))
            {
                //objectToAdd = Deserialize(type, value);
                //objectToAdd = MatrixxXmlSerializer.Instance.Deserialize(type, matrixxName);
            }
            if (type == typeof(Guid) || type == typeof(Guid?))
            {
                objectToAdd = new Guid(value);
            }
            else if (type == typeof(int) || type == typeof(int?))
            {
                objectToAdd = int.Parse(value);
            }
            else if (type == typeof(string))
            {
                objectToAdd = value;
            }
            else if (type == typeof(decimal) || type == typeof(decimal?))
            {
                objectToAdd = decimal.Parse(value);
            }
            else if (type == typeof(bool) || type == typeof(bool?))
            {
                objectToAdd = bool.Parse(value);
            }
            else if (type == typeof(long) || type == typeof(long?))
            {
                objectToAdd = long.Parse(value);
            }
            else if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                objectToAdd = DateTime.Parse(value).ToUniversalTime();
            }
            return objectToAdd;
        }
    }

    //public sealed class MatrixxXmlSerializer 
    //{
    //    private readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, XmlSerializer>> Serializers = new ConcurrentDictionary<Type, ConcurrentDictionary<string, XmlSerializer>>();

    //    private static readonly MatrixxXmlSerializer _instance = new MatrixxXmlSerializer();

    //    public static MatrixxXmlSerializer Instance
    //    {
    //        get { return _instance; }
    //    }

    //    private MatrixxXmlSerializer()
    //    {
    //    }

    //    #region Serializer Functions
    //    public void Serialize(XmlWriter xmlWriter, Type type, object input, string propertyName)
    //    {
    //        var serializer = InitSerializer(type, propertyName);

    //        var nameSpaces = new XmlSerializerNamespaces();
    //        nameSpaces.Add(string.Empty, string.Empty);

    //        serializer.Serialize(xmlWriter, input, nameSpaces);
    //    }

    //    public void Serialize(XmlWriter xmlWriter, Type type, object input)
    //    {
    //        var classAtt = Attribute.GetCustomAttribute(type, typeof(MatrixxContractAttribute));

    //        var serializer = InitSerializer(classAtt, type);

    //        var nameSpaces = new XmlSerializerNamespaces();
    //        nameSpaces.Add(string.Empty, string.Empty);

    //        serializer.Serialize(xmlWriter, input, nameSpaces);
    //    }

    //    private string GetKey(Type type, string propertyName)
    //    {
    //        return string.IsNullOrEmpty(propertyName) ? type.Name : propertyName;
    //    }

    //    private XmlSerializer InitSerializer(Type type, string propertyName)
    //    {
    //        var serializerDict = GetSerializerDictionaryByType(type);

    //        string key = GetKey(type, propertyName);

    //        return GetOrAddSerializer(serializerDict, type, propertyName, key);
    //    }

    //    private XmlSerializer GetOrAddSerializer(ConcurrentDictionary<string, XmlSerializer> serializerDict, Type type, string propertyName, string key)
    //    {
    //        return serializerDict.GetOrAdd(key,(!string.IsNullOrEmpty(propertyName))
    //                ? new XmlSerializer(type, new XmlRootAttribute { ElementName = propertyName })
    //                : new XmlSerializer(type));
    //    }

    //    private ConcurrentDictionary<string, XmlSerializer> GetSerializerDictionaryByType(Type type)
    //    {
    //        return Serializers.GetOrAdd(type, new ConcurrentDictionary<string, XmlSerializer>());
    //    }


    //    private XmlSerializer InitSerializer(Attribute classAtt, Type type)
    //    {
    //        var serializerDict = GetSerializerDictionaryByType(type);
    //        string propertyName = (classAtt != null) ? ((MatrixxContractAttribute)classAtt).Name : string.Empty;
    //        string key = GetKey(type, propertyName);

    //        return GetOrAddSerializer(serializerDict, type, propertyName, key);

    //    }

    //    public string Serialize(Type type, object input)
    //    {
    //        string result = string.Empty;

    //        var classAtt = Attribute.GetCustomAttribute(type, typeof(MatrixxContractAttribute));


    //        var nameSpaces = new XmlSerializerNamespaces();
    //        nameSpaces.Add(string.Empty, string.Empty);

    //        using (var memoryStream = new MemoryStream())
    //        {
    //            var serializer = InitSerializer(classAtt, type);

    //            serializer.Serialize(memoryStream, input, nameSpaces);
    //            result = Encoding.Default.GetString(memoryStream.ToArray());
    //        }
    //        return result;
    //    }

    //    public string Serialize<T>(T input) where T : new()
    //    {
    //        Type type = typeof(T);

    //        return Serialize(type, input);
    //    }
    //    #endregion

    //    #region Deserializer Functions

    //    public object Deserialize(XmlReader xmlReader, Type type, string propertyName)
    //    {
    //        var serializer = InitSerializer(type, propertyName);

    //        var nameSpaces = new XmlSerializerNamespaces();
    //        nameSpaces.Add(string.Empty, string.Empty);
    //        return serializer.Deserialize(xmlReader);
    //    }

    //    public object Deserialize(Type type, string input)
    //    {
    //        object result = null;

    //        var classAtt = Attribute.GetCustomAttribute(type, typeof(MatrixxContractAttribute));

    //        var serializer = InitSerializer(classAtt, type);
    //        var nameSpaces = new XmlSerializerNamespaces();
    //        nameSpaces.Add(string.Empty, string.Empty);

    //        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(input));

    //        try
    //        {

    //            result = serializer.Deserialize(memoryStream);
    //        }
    //        catch (Exception ex)
    //        {
    //            if (type.IsSubclassOf(typeof(MatrixxResponse)))
    //            {
    //                var resultResponse = Activator.CreateInstance(type) as MatrixxResponse;
    //                resultResponse.Text = ex.Message;
    //                resultResponse.Code = 500;
    //                result = resultResponse;
    //            }
    //            else
    //            {
    //                result = null;
    //            }
    //        }
    //        finally
    //        {
    //            memoryStream.Flush();
    //            memoryStream.Close();
    //            memoryStream.Dispose();
    //        }

    //        return result;
    //    }

    //    public T Deserialize<T>(string input) where T : new()
    //    {
    //        Type type = typeof(T);

    //        return (T)Deserialize(type, input);
    //    }
    //    #endregion


    //}
}
