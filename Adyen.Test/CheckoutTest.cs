﻿#region License
/*
 *                       ######
 *                       ######
 * ############    ####( ######  #####. ######  ############   ############
 * #############  #####( ######  #####. ######  #############  #############
 *        ######  #####( ######  #####. ######  #####  ######  #####  ######
 * ###### ######  #####( ######  #####. ######  #####  #####   #####  ######
 * ###### ######  #####( ######  #####. ######  #####          #####  ######
 * #############  #############  #############  #############  #####  ######
 *  ############   ############  #############   ############  #####  ######
 *                                      ######
 *                               #############
 *                               ############
 *
 * Adyen Dotnet API Library
 *
 * Copyright (c) 2020 Adyen B.V.
 * This file is open source and available under the MIT license.
 * See the LICENSE file for more info.
 */
#endregion

using Adyen.Model.ApplicationInformation;
using Adyen.Model.Checkout;
using Adyen.Model.Checkout.Details;
using Adyen.Model.Checkout.Action;
using Adyen.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using static Adyen.Model.Checkout.PaymentResponse;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Adyen.Test
{
    [TestClass]
    public class CheckoutTest : BaseTest
    {
        /// <summary>
        /// Tests successful checkout client Test URL generation.
        /// </summary>
        [TestMethod]
        public void CheckoutEndpointTestEnvironmentSuccessTest()
        {
            var config = new Config();
            var client = new Client(config);
            client.SetEnvironment(Model.Enum.Environment.Test, "companyUrl");
            Assert.AreEqual(config.CheckoutEndpoint, @"https://checkout-test.adyen.com");
            Assert.AreEqual(config.Endpoint, @"https://pal-test.adyen.com");
        }

        /// <summary>
        /// Tests successful checkout client Live URL generation.
        /// </summary>
        [TestMethod]
        public void CheckoutEndpointLiveEnvironmentSuccessTest()
        {
            var config = new Config();
            var client = new Client(config);
            client.SetEnvironment(Model.Enum.Environment.Live, "companyUrl");
            Assert.AreEqual(config.CheckoutEndpoint, @"https://companyUrl-checkout-live.adyenpayments.com/checkout");
            Assert.AreEqual(config.Endpoint, @"https://companyUrl-pal-live.adyenpayments.com");
        }

        /// <summary>
        /// Tests unsuccessful checkout client Live URL generation.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "Missing liveEndpointUrlPrefix for endpoint generation")]
        public void CheckoutEndpointLiveErrorTest()
        {
            var config = new Config();
            var client = new Client(config);
            client.SetEnvironment(Model.Enum.Environment.Live);
        }

        /// <summary>
        /// Test success flow for
        /// POST /payments
        /// </summary>
        [TestMethod]
        public void PaymentsAdditionalDataParsingTest()
        {
            var paymentRequest = CreatePaymentRequestCheckout();
            var client = CreateMockTestClientApiKeyBasedRequest("Mocks/checkout/payments-success.json");
            var checkout = new Checkout(client);
            var paymentResponse = checkout.Payments(paymentRequest);
            Assert.AreEqual("8535296650153317", paymentResponse.PspReference);
            Assert.AreEqual(ResultCodeEnum.Authorised, paymentResponse.ResultCode);
            Assert.IsNotNull(paymentResponse.AdditionalData);
            Assert.AreEqual(9, paymentResponse.AdditionalData.Count);
            Assert.AreEqual("8/2018", paymentResponse.AdditionalData["expiryDate"]);
            Assert.AreEqual("GREEN", paymentResponse.AdditionalData["fraudResultType"]);
            Assert.AreEqual("411111", paymentResponse.AdditionalData["cardBin"]);
            Assert.AreEqual("1111", paymentResponse.AdditionalData["cardSummary"]);
            Assert.AreEqual("false", paymentResponse.AdditionalData["fraudManualReview"]);
            Assert.AreEqual("Default", paymentResponse.AdditionalData["aliasType"]);
            Assert.AreEqual("H167852639363479", paymentResponse.AdditionalData["alias"]);
            Assert.AreEqual("visa", paymentResponse.AdditionalData["cardPaymentMethod"]);
            Assert.AreEqual("NL", paymentResponse.AdditionalData["cardIssuingCountry"]);
        }

        /// <summary>
        /// Test success flow for async
        /// POST /payments
        /// </summary>
        [TestMethod]
        public async Task PaymentsAsyncAdditionalDataParsingTest()
        {
            var paymentRequest = CreatePaymentRequestCheckout();
            var client = CreateAsyncMockTestClientApiKeyBasedRequest("Mocks/checkout/payments-success.json");
            var checkout = new Checkout(client);
            var paymentResponse = await checkout.PaymentsAsync(paymentRequest);
            Assert.AreEqual("8535296650153317", paymentResponse.PspReference);
            Assert.AreEqual(ResultCodeEnum.Authorised, paymentResponse.ResultCode);
            Assert.IsNotNull(paymentResponse.AdditionalData);
            Assert.AreEqual(9, paymentResponse.AdditionalData.Count);
            Assert.AreEqual("8/2018", paymentResponse.AdditionalData["expiryDate"]);
            Assert.AreEqual("GREEN", paymentResponse.AdditionalData["fraudResultType"]);
            Assert.AreEqual("411111", paymentResponse.AdditionalData["cardBin"]);
            Assert.AreEqual("1111", paymentResponse.AdditionalData["cardSummary"]);
            Assert.AreEqual("false", paymentResponse.AdditionalData["fraudManualReview"]);
            Assert.AreEqual("Default", paymentResponse.AdditionalData["aliasType"]);
            Assert.AreEqual("H167852639363479", paymentResponse.AdditionalData["alias"]);
            Assert.AreEqual("visa", paymentResponse.AdditionalData["cardPaymentMethod"]);
            Assert.AreEqual("NL", paymentResponse.AdditionalData["cardIssuingCountry"]);
        }

        /// <summary>
        /// Test success flow for Apple Pay
        /// POST /payments
        /// </summary>
        [TestMethod]
        public void PaymentsApplePayTest()
        {
            var paymentRequest = CreateApplePayPaymentRequestCheckout();
            var client = CreateMockTestClientApiKeyBasedRequest("Mocks/checkout/payments-applepay-success.json");
            var checkout = new Checkout(client);
            var paymentResponse = checkout.Payments(paymentRequest);
            Assert.AreEqual("9035798957043214", paymentResponse.PspReference);
            Assert.AreEqual(ResultCodeEnum.Authorised, paymentResponse.ResultCode);
        }

        /// <summary>
        /// Test success flow for Google Pay
        /// POST /payments
        /// </summary>
        [TestMethod]
        public void PaymentsGooglePayTest()
        {
            var paymentRequest = CreateGooglePayPaymentRequestCheckout();
            var client = CreateMockTestClientApiKeyBasedRequest("Mocks/checkout/payments-googlepay-success.json");
            var checkout = new Checkout(client);
            var paymentResponse = checkout.Payments(paymentRequest);
            Assert.AreEqual("9035798960987345", paymentResponse.PspReference);
            Assert.AreEqual(ResultCodeEnum.Authorised, paymentResponse.ResultCode);
        }

        /// <summary>
        /// Test success flow for 3DS2
        /// POST /payments
        /// </summary>
        [TestMethod]
        public void Payments3DS2Test()
        {
            var payment3DS2Request = CreatePaymentRequest3DS2();
            var client = CreateMockTestClientApiKeyBasedRequest("Mocks/checkout/payments-3DS2-IdentifyShopper.json");
            var checkout = new Checkout(client);
            var paymentResponse = checkout.Payments(payment3DS2Request);
            Assert.AreEqual(paymentResponse.ResultCode, ResultCodeEnum.IdentifyShopper);
            Assert.IsFalse(string.IsNullOrEmpty(paymentResponse.PaymentData));
        }

        /// <summary>
        /// Test 3ds success flow for
        /// POST  /payments/result
        /// </summary>
        [TestMethod]
        public void PaymentsAuthorise3ds2ResultSuccessTest()
        {
            var paymentResultRequest = CreatePaymentResultRequest();
            var client = CreateMockTestClientApiKeyBasedRequest("Mocks/threedsecure2/authorise3ds2-success.json");
            var checkout = new Checkout(client);
            var paymentResultResponse = checkout.PaymentsResult(paymentResultRequest);
            Assert.IsNotNull(paymentResultResponse.AdditionalData);
            Assert.AreEqual(paymentResultResponse.AdditionalData["cvcResult"], "1 Matches");
            Assert.AreEqual(paymentResultResponse.MerchantReference, "your_merchantReference");
            Assert.AreEqual(paymentResultResponse.ResultCode, PaymentResultResponse.ResultCodeEnum.Authorised);
        }

        /// <summary>
        /// Test error flow for
        /// POST /payments
        /// </summary>
        [TestMethod]
        public void PaymentsErrorTest()
        {
            var paymentMethodsRequest = CreatePaymentRequestCheckout();
            var client = CreateMockTestClientApiKeyBasedRequest("Mocks/checkout/payments-error-invalid-data-422.json");
            var checkout = new Checkout(client);
            var paymentResponse = checkout.Payments(paymentMethodsRequest);
            Assert.IsNull(paymentResponse.PspReference);
        }

        /// <summary>
        /// Test success flow for
        /// POST /payments/details
        /// </summary>
        [TestMethod]
        public void PaymentDetailsTest()
        {
            var detailsRequest = CreateDetailsRequest();
            detailsRequest.Details = new PaymentCompletionDetails(payload: "Ab02b4c0!BQABAgBQn96RxfJHpp2RXhqQBuhQFWgE...gfGHb4IZSP4IpoCC2==RXhqQBuhQ");
            var client = CreateMockTestClientApiKeyBasedRequest("Mocks/checkout/paymentsdetails-success.json");
            var checkout = new Checkout(client);
            var paymentResponse = checkout.PaymentDetails(detailsRequest);
            Assert.AreEqual("8515232733321252", paymentResponse.PspReference);
        }

        /// <summary>
        /// Test success flow for async
        /// POST /payments/details
        /// </summary>
        [TestMethod]
        public async Task PaymentDetailsAsyncTest()
        {
            var detailsRequest = CreateDetailsRequest();
            detailsRequest.Details = new PaymentCompletionDetails(payload: "Ab02b4c0!BQABAgBQn96RxfJHpp2RXhqQBuhQFWgE...gfGHb4IZSP4IpoCC2==RXhqQBuhQ");
            var client = CreateAsyncMockTestClientApiKeyBasedRequest("Mocks/checkout/paymentsdetails-success.json");
            var checkout = new Checkout(client);
            var paymentResponse = await checkout.PaymentDetailsAsync(detailsRequest);
            Assert.AreEqual("8515232733321252", paymentResponse.PspReference);
        }

        /// <summary>
        /// Test error flow for
        /// POST /payments/details
        /// </summary>
        [TestMethod]
        public void PaymentDetailsErrorTest()
        {
            var detailsRequest = CreateDetailsRequest();
            detailsRequest.Details = new PaymentCompletionDetails(payload: "Ab02b4c0!BQABAgBQn96RxfJHpp2RXhqQBuhQFWgE...gfGHb4IZSP4IpoCC2==RXhqQBuhQ");
            var client = CreateMockTestClientApiKeyBasedRequest("Mocks/checkout/paymentsdetails-error-invalid-data-422.json");
            var checkout = new Checkout(client);
            var paymentResponse = checkout.PaymentDetails(detailsRequest);
            Assert.IsNull(paymentResponse.ResultCode);
        }

        /// <summary>
        /// Test success deserialization for
        /// POST /payments/details
        /// </summary>
        [TestMethod]
        public void PaymentDetailsResponseDeserializeTest()
        {
            var detailsRequest = CreateDetailsRequest();
            detailsRequest.Details = new PaymentCompletionDetails(payload: "Ab02b4c0!BQABAgBQn96RxfJHpp2RXhqQBuhQFWgE...gfGHb4IZSP4IpoCC2==RXhqQBuhQ");
            var client = CreateMockTestClientApiKeyBasedRequest("Mocks/checkout/paymentsdetails-action-success.json");
            var checkout = new Checkout(client);
            var paymentResponse = checkout.PaymentDetails(detailsRequest);
            Assert.AreEqual(paymentResponse.PspReference, "8515232733321252");
            Assert.AreEqual(paymentResponse.ResultCode, PaymentDetailsResponse.ResultCodeEnum.Authorised);
        }

        /// <summary>
        /// Test success flow for
        /// POST /paymentMethods
        /// </summary>
        [TestMethod]
        public void PaymentMethodsTest()
        {
            var paymentMethodsRequest = CreatePaymentMethodRequest("YourMerchantAccount");
            var client = CreateMockTestClientApiKeyBasedRequest("Mocks/checkout/paymentmethods-success.json");
            var checkout = new Checkout(client);
            var paymentMethodsResponse = checkout.PaymentMethods(paymentMethodsRequest);
            Assert.AreEqual(paymentMethodsResponse.PaymentMethods.Count, 65);
        }

        /// <summary>
        /// Test success flow for
        /// POST /paymentMethods
        /// </summary>
        [TestMethod]
        public void PaymentMethodsStoreValueTest()
        {
            var paymentMethodsRequest = CreatePaymentMethodRequest("YourMerchantAccount");
            paymentMethodsRequest.Store = "MerchantStore";
            Assert.AreEqual(paymentMethodsRequest.Store, "MerchantStore");
        }

        /// <summary>
        /// Test success flow for async
        /// POST /paymentMethods
        /// </summary>
        [TestMethod]
        public async Task PaymentMethodsAsyncTest()
        {
            var paymentMethodsRequest = CreatePaymentMethodRequest("YourMerchantAccount");
            var client = CreateAsyncMockTestClientApiKeyBasedRequest("Mocks/checkout/paymentmethods-success.json");
            var checkout = new Checkout(client);
            var paymentMethodsResponse = await checkout.PaymentMethodsAsync(paymentMethodsRequest);
            Assert.AreEqual(paymentMethodsResponse.PaymentMethods.Count, 65);
        }

        /// <summary>
        /// Test error flow for
        /// POST /paymentMethods
        /// </summary>
        [TestMethod]
        public void PaymentMethodsErrorTest()
        {
            var paymentMethodsRequest = CreatePaymentMethodRequest("YourMerchantAccount");
            var client = CreateMockTestClientApiKeyBasedRequest("Mocks/checkout/paymentmethods-error-forbidden-403.json");
            var checkout = new Checkout(client);
            var paymentMethodsResponse = checkout.PaymentMethods(paymentMethodsRequest);
            Assert.IsNull(paymentMethodsResponse.PaymentMethods);
        }

        /// <summary>
        /// Test success flow including brands check for
        /// POST /paymentMethods
        /// </summary>
        [TestMethod]
        public void PaymentMethodsWithBrandsTest()
        {
            var paymentMethodsRequest = CreatePaymentMethodRequest("YourMerchantAccount");
            var client = CreateMockTestClientApiKeyBasedRequest("Mocks/checkout/paymentmethods-brands-success.json");
            var checkout = new Checkout(client);
            var paymentMethodsResponse = checkout.PaymentMethods(paymentMethodsRequest);
            Assert.AreEqual(paymentMethodsResponse.PaymentMethods.Count, 7);
            Assert.AreEqual(paymentMethodsResponse.PaymentMethods[0].Brands.Count, 5);
            Assert.AreEqual(paymentMethodsResponse.PaymentMethods[0].Brands[0], "visa");
            Assert.AreEqual(paymentMethodsResponse.PaymentMethods[0].Brands[1], "mc");
            Assert.AreEqual(paymentMethodsResponse.PaymentMethods[0].Brands[2], "amex");
            Assert.AreEqual(paymentMethodsResponse.PaymentMethods[0].Brands[3], "bcmc");
            Assert.AreEqual(paymentMethodsResponse.PaymentMethods[0].Brands[4], "maestro");
        }

        /// <summary>
        /// Test flow without including brands check for
        /// POST /paymentMethods
        /// </summary>
        [TestMethod]
        public void PaymentMethodsWithoutBrandsTest()
        {
            var paymentMethodsRequest = CreatePaymentMethodRequest("YourMerchantAccount");
            var client = CreateMockTestClientApiKeyBasedRequest("Mocks/checkout/paymentmethods-without-brands-success.json");
            var checkout = new Checkout(client);
            var paymentMethodsResponse = checkout.PaymentMethods(paymentMethodsRequest);
            Assert.AreEqual(paymentMethodsResponse.PaymentMethods.Count, 50);
        }


        /// <summary>
        /// Test success flow for
        /// POST  /paymentSession
        /// </summary>
        [TestMethod]
        public void PaymentSessionSuccessTest()
        {
            var paymentSessionRequest = CreatePaymentSessionRequest();
            var client = CreateMockTestClientApiKeyBasedRequest("Mocks/checkout/paymentsession-sucess.json");
            var checkout = new Checkout(client);
            var paymentSessionResponse = checkout.PaymentSession(paymentSessionRequest);
            Assert.IsNotNull(paymentSessionResponse.PaymentSession);
        }

        /// <summary>
        /// Test success flow for async
        /// POST  /paymentSession
        /// </summary>
        [TestMethod]
        public async Task PaymentSessionAsyncSuccessTest()
        {
            var paymentSessionRequest = CreatePaymentSessionRequest();
            var client = CreateAsyncMockTestClientApiKeyBasedRequest("Mocks/checkout/paymentsession-sucess.json");
            var checkout = new Checkout(client);
            var paymentSessionResponse = await checkout.PaymentSessionAsync(paymentSessionRequest);
            Assert.IsNotNull(paymentSessionResponse.PaymentSession);
        }

        /// <summary>
        /// Test success flow for
        /// POST  /paymentSession
        /// </summary>
        [TestMethod]
        public void PaymentSessionErrorTest()
        {
            var paymentSessionRequest = CreatePaymentSessionRequest();
            var client = CreateMockTestClientApiKeyBasedRequest("Mocks/checkout/paymentsession-error-invalid-data-422.json");
            var checkout = new Checkout(client);
            var paymentSessionResponse = checkout.PaymentSession(paymentSessionRequest);
            Assert.IsNull(paymentSessionResponse.PaymentSession);
        }

        /// <summary>
        /// Test success flow for
        /// POST  /payments/result
        /// </summary>
        [TestMethod]
        public void PaymentsResultSuccessTest()
        {
            var paymentResultRequest = CreatePaymentResultRequest();
            var client = CreateMockTestClientApiKeyBasedRequest("Mocks/checkout/paymentsresult-success.json");
            var checkout = new Checkout(client);
            var paymentResultResponse = checkout.PaymentsResult(paymentResultRequest);
            Assert.AreEqual(paymentResultResponse.ResultCode, PaymentResultResponse.ResultCodeEnum.Authorised);
        }

        /// <summary>
        /// Test success flow for async
        /// POST  /payments/result
        /// </summary>
        [TestMethod]
        public async Task PaymentsResultAsyncSuccessTest()
        {
            var paymentResultRequest = CreatePaymentResultRequest();
            var client = CreateAsyncMockTestClientApiKeyBasedRequest("Mocks/checkout/paymentsresult-success.json");
            var checkout = new Checkout(client);
            var paymentResultResponse = await checkout.PaymentsResultAsync(paymentResultRequest);
            Assert.AreEqual(paymentResultResponse.ResultCode, PaymentResultResponse.ResultCodeEnum.Authorised);
        }

        /// <summary>
        /// Test success flow for
        /// POST  /payments/result
        /// </summary>
        [TestMethod]
        public void PaymentsResultErrorTest()
        {
            var paymentResultRequest = CreatePaymentResultRequest();
            var client = CreateMockTestClientApiKeyBasedRequest("Mocks/checkout/paymentsresult-error-invalid-data-payload-422.json");
            var checkout = new Checkout(client);
            var paymentResultResponse = checkout.PaymentsResult(paymentResultRequest);
            Assert.IsNull(paymentResultResponse.ResultCode);
        }

        [TestMethod]

        public void ApplicationInfoTest()
        {
            ApplicationInfo applicationInfo = new ApplicationInfo();
            Assert.AreEqual(applicationInfo.AdyenLibrary.Name, Constants.ClientConfig.LibName);
            Assert.AreEqual(applicationInfo.AdyenLibrary.Version, Constants.ClientConfig.LibVersion);

        }
        [TestMethod]
        public void PaymentRequestApplicationInfoTest()
        {
            var paymentRequest = CreatePaymentRequestCheckout();
            var name = paymentRequest.ApplicationInfo.AdyenLibrary.Name;
            var version = paymentRequest.ApplicationInfo.AdyenLibrary.Version;
            Assert.AreEqual(version, Constants.ClientConfig.LibVersion);
            Assert.AreEqual(name, Constants.ClientConfig.LibName);
        }

        [TestMethod]
        public void PaymentRequestAppInfoExternalTest()
        {
            var externalPlatform = new Model.ApplicationInformation.ExternalPlatform();
            var merchantApplication = new Model.ApplicationInformation.CommonField();
            externalPlatform.Integrator = "TestExternalPlatformIntegration";
            externalPlatform.Name = "TestExternalPlatformName";
            externalPlatform.Version = "TestExternalPlatformVersion";
            merchantApplication.Name = "MerchantApplicationName";
            merchantApplication.Version = "MerchantApplicationVersion";
            var paymentRequest = CreatePaymentRequestCheckout();
            paymentRequest.ApplicationInfo.ExternalPlatform = externalPlatform;
            paymentRequest.ApplicationInfo.MerchantApplication = merchantApplication;
            Assert.AreEqual(paymentRequest.ApplicationInfo.ExternalPlatform.Integrator, "TestExternalPlatformIntegration");
            Assert.AreEqual(paymentRequest.ApplicationInfo.ExternalPlatform.Name, "TestExternalPlatformName");
            Assert.AreEqual(paymentRequest.ApplicationInfo.ExternalPlatform.Version, "TestExternalPlatformVersion");
            Assert.AreEqual(paymentRequest.ApplicationInfo.MerchantApplication.Name, "MerchantApplicationName");
            Assert.AreEqual(paymentRequest.ApplicationInfo.MerchantApplication.Version, "MerchantApplicationVersion");
        }

     
        [TestMethod]
        public void PaymentsOriginTest()
        {
            var paymentMethodsRequest = CreatePaymentRequestCheckout();
            paymentMethodsRequest.Origin = "https://localhost:8080";
            Assert.AreEqual(paymentMethodsRequest.Origin, "https://localhost:8080");
        }

        /// <summary>
        /// Test CreatePaymentLinkRequest
        /// POST  /payments/result
        /// </summary>
        [TestMethod]
        public void CreatePaymentLinkSuccess()
        {
            var createPaymentLinkRequest = new CreatePaymentLinkRequest(store: "TheDemoStore", amount: new Amount(currency: "EUR", 1000), merchantAccount: "MerchantAccount", reference: "reference");
            Assert.AreEqual(createPaymentLinkRequest.Store, "TheDemoStore");
        }

        /// <summary>
        /// Test success flow for
        /// POST  /payments/result
        /// </summary>
        [TestMethod]
        public void PaymentLinksSuccess()
        {
            var client = CreateMockTestClientApiKeyBasedRequest("Mocks/checkout/payment-links-success.json");
            var checkout = new Checkout(client);
            var createPaymentLinkRequest = new CreatePaymentLinkRequest(amount: new Amount(currency: "EUR", 1000), merchantAccount: "MerchantAccount", reference: "YOUR_ORDER_NUMBER");
            var paymentLinksResponse = checkout.PaymentLinks(createPaymentLinkRequest);
            Assert.AreEqual(paymentLinksResponse.Url, "https://checkoutshopper-test.adyen.com/checkoutshopper/payByLink.shtml?d=YW1vdW50TWlub3JW...JRA");
            Assert.AreEqual(paymentLinksResponse.ExpiresAt, "2019-12-17T10:05:29Z");
            Assert.AreEqual(paymentLinksResponse.Reference, "YOUR_ORDER_NUMBER");
            Assert.IsNotNull(paymentLinksResponse.Amount);
        }

        /// <summary>
        /// Test success flow for creation of a payment link with recurring payment
        /// POST /paymentLinks
        /// </summary>
        [TestMethod]
        public void CreateRecurringPaymentLinkSuccessTest()
        {
            var client = CreateMockTestClientApiKeyBasedRequest("Mocks/checkout/paymentlinks-recurring-payment-success.json");
            var checkout = new Checkout(client);

            var createPaymentLinkRequest = new CreatePaymentLinkRequest(amount: new Amount(currency: "EUR", 100), merchantAccount: "MerchantAccount", reference: "REFERENCE_NUMBER")
            {
                CountryCode = "GR",
                ShopperLocale = "GR",
                ShopperReference = "ShopperReference",
                StorePaymentMethod = true,
                RecurringProcessingModel = CreatePaymentLinkRequest.RecurringProcessingModelEnum.Subscription
            };

            var paymentLinksResponse = checkout.PaymentLinks(createPaymentLinkRequest);

            Assert.AreEqual(createPaymentLinkRequest.Reference, paymentLinksResponse.Reference);
            Assert.AreEqual("https://checkoutshopper-test.adyen.com/checkoutshopper/payByLink.shtml?d=YW1vdW50TWlub3JW...JRA", paymentLinksResponse.Url);
            Assert.AreEqual(createPaymentLinkRequest.Amount.Currency, paymentLinksResponse.Amount.Currency);
            Assert.AreEqual(createPaymentLinkRequest.Amount.Value, paymentLinksResponse.Amount.Value);
        }

        /// <summary>
        /// Test success flow for multibanco
        /// Post /payments 
        /// </summary>
        [TestMethod]
        public void MultibancoPaymentSuccessMockedTest()
        {
            var client = CreateMockTestClientRequest("Mocks/checkout/paymentsresult-multibanco-success.json");
            var checkout = new Checkout(client);
            var paymentRequest = CreatePaymentRequestCheckout();
            var paymentResponse = checkout.Payments(paymentRequest);
            var paymentResponseAction = (CheckoutVoucherAction)paymentResponse.Action;
            Assert.AreEqual(paymentResponseAction.PaymentMethodType, "multibanco");
            Assert.AreEqual(paymentResponseAction.ExpiresAt, "01/12/2020 09:37:49");
            Assert.AreEqual(paymentResponseAction.Reference, "501 422 944");
            Assert.AreEqual(paymentResponseAction.Entity, "12101");
        }

        /// <summary>
        /// Test RiskData - Clientdata flow for
        /// POST /payments
        /// </summary>
        [TestMethod]
        public void PaymentClientdataParsingTest()
        {
            var paymentRequest = CreatePaymentRequestCheckout();
            var riskdata = new RiskData
            {
                ClientData = "IOfW3k9G2PvXFu2j"
            };
            paymentRequest.RiskData = riskdata;
            Assert.AreEqual(paymentRequest.RiskData.ClientData, "IOfW3k9G2PvXFu2j");
        }

        /// <summary>
        /// Test success flow for paypal
        /// Post /payments 
        /// </summary>
        [TestMethod]
        public void PaypalPaymentSuccessTest()
        {
            var client = CreateMockTestClientRequest("Mocks/checkout/payments-success-paypal.json");
            var checkout = new Checkout(client);
            var paymentRequest = CreatePaymentRequestCheckout();
            var paymentResponse = checkout.Payments(paymentRequest);
            Assert.IsTrue(paymentResponse.Action is CheckoutSDKAction);
            var result = (CheckoutSDKAction)paymentResponse.Action;
            Assert.AreEqual("EC-42N19135GM6949000", result.SdkData["orderID"]);
            Assert.AreEqual("Ab02b4c0!BQABAgARb1TvUJa4nwS0Z1nOmxoYfD9+z...", result.PaymentData);
            Assert.AreEqual("paypal", result.PaymentMethodType);
        }

        [TestMethod]
        public void ApplePayDetailsDeserializationTest()
        {
            var json = "{\"type\": \"applepay\",\"applePayToken\": \"VNRWtuNlNEWkRCSm1xWndjMDFFbktkQU...\"}";
            var result = Util.JsonOperation.Deserialize<IPaymentMethodDetails>(json);
            Assert.IsTrue(result is ApplePayDetails);
            Assert.AreEqual(result.Type, "applepay");
        }

        [TestMethod]
        public void BlikDetailsDeserializationTest()
        {
            var json = "{\"amount\":{\"value\":1000,\"currency\":\"USD\"},\"merchantAccount\":\"MerchantAccountTest\",\"paymentMethod\":{\"blikCode\":\"blikCode\",\"type\":\"blik\"},\"reference\":\"Your order number\",\"returnUrl\":\"https://your-company.com/...\",\"applicationInfo\":{\"adyenLibrary\":{\"name\":\"adyen-java-api-library\",\"version\":\"10.1.0\"}}}";
            var paymentRequest = JsonConvert.DeserializeObject<PaymentRequest>(json);
            Assert.IsTrue(paymentRequest.PaymentMethod is BlikDetails);
            Assert.AreEqual(paymentRequest.PaymentMethod.Type, BlikDetails.Blik);
        }

        [TestMethod]
        public void DragonpayDetailsDeserializationTest()
        {
            var json = "{\"amount\":{\"value\":1000,\"currency\":\"USD\"},\"merchantAccount\":\"MerchantAccountTest\",\"paymentMethod\":{\"issuer\":\"issuer\",\"shopperEmail\":\"test@test.com\",\"type\":\"dragonpay_ebanking\"},\"reference\":\"Your order number\",\"returnUrl\":\"https://your-company.com/...\",\"applicationInfo\":{\"adyenLibrary\":{\"name\":\"adyen-java-api-library\",\"version\":\"10.1.0\"}}}";
            var paymentRequest = Util.JsonOperation.Deserialize<PaymentRequest>(json);
            Assert.IsTrue(paymentRequest.PaymentMethod is DragonpayDetails);
            Assert.AreEqual(paymentRequest.PaymentMethod.Type, DragonpayDetails.EBanking);
        }

        [TestMethod]
        public void LianLianPayDetailsDeserializationTest()
        {
            var json = "{\"amount\":{\"value\":1000,\"currency\":\"USD\"},\"merchantAccount\":\"MerchantAccountTest\",\"paymentMethod\":{\"telephoneNumber\":\"telephone\",\"type\":\"lianlianpay_ebanking_credit\"},\"reference\":\"Your order number\",\"returnUrl\":\"https://your-company.com/...\",\"applicationInfo\":{\"adyenLibrary\":{\"name\":\"adyen-java-api-library\",\"version\":\"10.1.0\"}}}";

            LianLianPayDetails lianLianPayDetails = new LianLianPayDetails();
            var paymentRequest = JsonConvert.DeserializeObject<PaymentRequest>(json);
            Assert.IsTrue(paymentRequest.PaymentMethod is LianLianPayDetails);
            Assert.AreEqual(paymentRequest.PaymentMethod.Type, LianLianPayDetails.EbankingCredit);
        }

        /// <summary>
        /// Test toJson() that includes the type in the action
        /// </summary>
        [TestMethod]
        public void PaymentsResponseToJsonTest()
        {
            var paymentRequest = CreatePaymentRequestCheckout();
            var client = CreateMockTestClientApiKeyBasedRequest("Mocks/checkout/paymentResponse-3DS-ChallengeShopper.json");
            var checkout = new Checkout(client);
            var paymentResponse = checkout.Payments(paymentRequest);
            var paymentResponseToJson = paymentResponse.ToJson();
            var jObject = JObject.Parse(paymentResponseToJson);
            Assert.AreEqual(jObject["action"]["type"], "threeDS2");
        }

        [TestMethod]
        public void StoredPaymentMethodsTest()
        {
            var client = CreateMockTestClientApiKeyBasedRequest("Mocks/checkout/paymentmethods-storedpaymentmethods.json");
            var checkout = new Checkout(client);
            var paymentMethodsRequest = new PaymentMethodsRequest(merchantAccount: "TestMerchant");
            var paymentMethodsResponse = checkout.PaymentMethods(paymentMethodsRequest);
            Assert.AreEqual(4, paymentMethodsResponse.StoredPaymentMethods.Count);
            Assert.AreEqual("NL32ABNA0515071439", paymentMethodsResponse.StoredPaymentMethods[0].Iban);
            Assert.AreEqual("Adyen", paymentMethodsResponse.StoredPaymentMethods[0].OwnerName);
            Assert.AreEqual("sepadirectdebit", paymentMethodsResponse.StoredPaymentMethods[0].Type);
        }

        /// <summary>
        /// Test if the fraud result are properly deseriazed
        /// POST /payments
        /// </summary>
        [TestMethod]
        public void FraudResultParsingTest()
        {
            var paymentRequest = CreatePaymentRequestCheckout();
            var client = CreateMockTestClientApiKeyBasedRequest("Mocks/checkout/payments-success.json");
            var checkout = new Checkout(client);
            var paymentResponse = checkout.Payments(paymentRequest);
            Assert.AreEqual(25, paymentResponse.FraudResult.AccountScore);
            var fraudResults = paymentResponse.FraudResult.Results;
            Assert.IsNotNull(fraudResults);
            Assert.AreEqual(11, fraudResults.Count);
            Assert.AreEqual("CardChunkUsage", fraudResults[0].FraudCheckResult.Name);
            Assert.AreEqual(0, fraudResults[0].FraudCheckResult.AccountScore);
            Assert.AreEqual(2, fraudResults[0].FraudCheckResult.CheckId);
            Assert.AreEqual("PaymentDetailUsage", fraudResults[1].FraudCheckResult.Name);
            Assert.AreEqual(0, fraudResults[1].FraudCheckResult.AccountScore);
            Assert.AreEqual(3, fraudResults[1].FraudCheckResult.CheckId);
        }
        /// <summary>
        /// Test if the fraud result are properly deseriazed
        /// POST /payments
        /// </summary>
        [TestMethod]
        public void ThreeDS2Test()
        {
            var paymentRequest = CreatePaymentRequestCheckout();
            var client = CreateMockTestClientApiKeyBasedRequest("Mocks/checkout/paymentResponse-3DS2-Action.json");
            var checkout = new Checkout(client);
            var paymentResponse = checkout.Payments(paymentRequest);
            var paymentResponseThreeDs2Action = (CheckoutThreeDS2Action)paymentResponse.Action;
            Assert.AreEqual(ResultCodeEnum.IdentifyShopper, paymentResponse.ResultCode);
            Assert.AreEqual("threeDS2", paymentResponseThreeDs2Action.Type);
        }
    }
}