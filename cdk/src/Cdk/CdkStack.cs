using Amazon.CDK;
using Amazon.CDK.AWS.ECR;

namespace Cdk
{
    public class CdkStack : Stack
    {
        internal CdkStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
	        var repository = new Repository(this, "cdk-test", new RepositoryProps
	        {
		        RepositoryName = "cdk_test_ecr_repo"
            });
        }
    }
}
