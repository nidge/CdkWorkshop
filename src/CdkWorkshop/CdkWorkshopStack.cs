// This is from the tutorial https://cdkworkshop.com/40-dotnet.html

using Amazon.CDK;
using Amazon.CDK.AWS.SNS;
using Amazon.CDK.AWS.Lambda;
using Cdklabs.DynamoTableViewer;
using Amazon.CDK.AWS.SNS.Subscriptions;
using Amazon.CDK.AWS.SQS;
using Constructs;
using Amazon.CDK.AWS.APIGateway;

namespace CdkWorkshop
{
    public class CdkWorkshopStack : Stack
    {
        internal CdkWorkshopStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            // Define a new Lambda resource
            var hello = new Function(this, "HelloHandler", new FunctionProps
            {
                Runtime = Runtime.NODEJS_14_X,  // Execution environment
                Code = Code.FromAsset("lambda"), // code loaded from the lambda directory
                Handler = "hello.handler" // file is "hello", function is "handler"
            });

            var helloWithCounter = new HitCounter(this, "HelloHitCounter", new HitCounterProps
            {
                Downstream = hello
            });

            // define an API Gateway REST API resource backed up by our "hello" function
            new LambdaRestApi(this, "EndPoint", new LambdaRestApiProps
            {
                Handler = helloWithCounter.Handler
            });

            // Defines a new TableViewer resource
            new TableViewer(this, "ViewerHitCount", new TableViewerProps
            {
                Title = "Hello Hits",
                Table = helloWithCounter.MyTable, 
                SortBy = "hits"
            });
        }
    }
}
