using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OCSServices.Matrixx.Api.Client.Contracts.Model.Offer;
using OCSServices.Matrixx.Api.Client.Contracts.Response.Offer;

namespace OCSServices.Matrixx.Agent.UT
{
    [TestClass]
    public class Subsciber_Tests
    {
        // Failing since 2016 because SubscriberBuilder is declared as internal
        //[TestMethod]
        //public void CraeteSubscriberOnMatrixx_BuildCreateSubscriberRequest_Test()
        //{
        //    //Business.Subscriber subscriberBuilder = new Business.Subscriber();
        //    Contracts.Subscriber.CreateSubscriberRequest request = new Contracts.Subscriber.CreateSubscriberRequest
        //    {
        //        BillingCycleId = "100",
        //        CreateWithStatus = 1,
        //        CrmProductId = Guid.Parse("dba97a27-4665-e611-89b0-06ac9d13ea4d"),
        //        MsisdnList = new List<string> { "31637000662" },
        //        ImsiList = new List<string> { "204070080652035", "204047910652035" },
        //        MembershipCodes = new List<string> { "GROUP_Teleena" }
        //    };
        //    Api.Client.Contracts.Request.MultiRequest result = null; // subscriberBuilder.BuildCreateSubscriberRequest(request);
        //    Api.Client.Contracts.Base.MatrixxXmlSerializer serializer = new Api.Client.Contracts.Base.MatrixxXmlSerializer();
        //    string value = serializer.Serialize(result);

        //    Assert.IsTrue(value.Contains("AccessNumberArray") && value.Contains(@"<value>31637000662</value>"));
        //}

        [TestMethod]
        public void FlexTopupWithNewOffer_BuildPurchaseOfferForSubscriberRequest_Serialize()
        {
            AddOfferToSubscriberResponse response = new AddOfferToSubscriberResponse
            {
                ObjectId = Guid.NewGuid().ToString(),
                Result = 0,
                ResultText = "OK",
                PurchaseInfoList = new PurchaseInfoCollection()
                {
                    Values = new List<PurchaseInfo>
                    {
                        new PurchaseInfo
                        {
                            Balances = new RequiredBalances { Values = new List<RequiredBalanceInfo> { new RequiredBalanceInfo { ResourceId = 9, TemplateId = 21024 } } },
                            ProductOfferId = 17,
                            ProductOfferVersion = 0,
                            ProductOfferExternalId = "IIJDataBundle",
                            ResourceId = 0
                        }
                    }
                }
            };

            Api.Client.Contracts.Base.MatrixxXmlSerializer serializer = new Api.Client.Contracts.Base.MatrixxXmlSerializer();
            string value = serializer.Serialize(response);

            Assert.IsTrue(value.Contains(@"<ResourceId>9</ResourceId>"));
        }

        [TestMethod]
        public void FlexTopupWithNewOffer_BuildPurchaseOfferForSubscriberRequest_Deserialize()
        {
            string xml = @"<MtxResponsePurchase>
    <PurchaseInfoArray>
        <MtxPurchaseInfo>
            <ProductOfferId>17</ProductOfferId>
            <ProductOfferVersion>0</ProductOfferVersion>
            <ExternalId>IIJDataBundle</ExternalId>
            <ResourceId>0</ResourceId>
            <OfferType>1</OfferType>
            <RequiredBalanceArray>
                <MtxRequiredBalanceInfo>
                    <TemplateId>21024</TemplateId>
                    <ResourceId>6</ResourceId>
                </MtxRequiredBalanceInfo>
            </RequiredBalanceArray>
        </MtxPurchaseInfo>
    </PurchaseInfoArray>
    <ObjectId>0-3-5-1454587</ObjectId>
    <ResourceIdArray>
        <value>0</value>
    </ResourceIdArray>
    <Result>0</Result>
    <ResultText>OK</ResultText>
</MtxResponsePurchase>";

            Api.Client.Contracts.Base.MatrixxXmlSerializer serializer = new Api.Client.Contracts.Base.MatrixxXmlSerializer();
            var value = serializer.Deserialize<AddOfferToSubscriberResponse>(xml);

            Assert.IsTrue(value.PurchaseInfoList.Values[0].Balances.Values[0].ResourceId == 6);
        }

        [TestMethod]
        public void MultiResponse_Deserialize_Message_Success()
        {
            #region Xml Message Response
            string xml = @"<MtxResponseMulti>
    <ResponseList>
        <MtxResponseCreate>
            <ObjectId>0-2-5-1285217</ObjectId>
            <Result>0</Result>
            <ResultText>OK</ResultText>
        </MtxResponseCreate>
        <MtxResponseCreate>
            <ObjectId>0-2-5-1285219</ObjectId>
            <Result>0</Result>
            <ResultText>OK</ResultText>
        </MtxResponseCreate>
        <MtxResponse>
            <Result>0</Result>
            <ResultText>OK</ResultText>
        </MtxResponse>
    </ResponseList>
    <Result>19</Result>
    <ResultText>Duplicate indexed key value.</ResultText>
</MtxResponseMulti>";
            #endregion

            Api.Client.Contracts.Base.MatrixxXmlSerializer serializer = new Api.Client.Contracts.Base.MatrixxXmlSerializer();
            var value = serializer.Deserialize<Api.Client.Contracts.Response.Multi.MultiResponse>(xml);

            Assert.IsTrue(value.Code == 19);
            Assert.IsTrue(value.ResponseCollection.ResponseList.Count == 3);
        }
    }
}
