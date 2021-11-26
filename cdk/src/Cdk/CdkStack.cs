using Amazon.CDK;
using Amazon.CDK.AWS.ECR;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.SQS;

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
            
            var queue = new Queue(this, "cdk-test-queue");

            var user = new User(this, "cdk-test-user", new UserProps
            {
                UserName = "cdk-test-user"
            });
            
            
            //this is not working ATM
            var taskRole = new Role(this, "cdk-test-cluster-user", new RoleProps
            {
                RoleName = "cdk-test-ecs-task-role",
                AssumedBy = user,
                ManagedPolicies = new IManagedPolicy[]
                {
                    new ManagedPolicy(this, "full-access-to-ecs", new ManagedPolicyProps
                    {
                        Description = "gives full access to ECS cluster",
                        ManagedPolicyName = "full-access-to-ecs",
                        Statements = new PolicyStatement[]
                        {
                            new PolicyStatement(new PolicyStatementProps
                            {
                                Effect = Effect.ALLOW,
                                Actions = new[]
                                {
                                    "ecr:GetAuthorizationToken",
                                    "ecr:BatchCheckLayerAvailability",
                                    "ecr:GetDownloadUrlForLayer",
                                    "ecr:BatchGetImage",
                                    "logs:CreateLogStream",
                                    "logs:PutLogEvents"
                                },
                                Resources = new[]
                                {
                                    "*"
                                }
                            })
                        }
                    })
                }
            });

            var taskDef = new FargateTaskDefinition(this, "cdk-test-ecs-task", new FargateTaskDefinitionProps
            {
                Cpu = 256,
                MemoryLimitMiB = 1024,
                Family = "cdk-test-tasks",
                TaskRole = taskRole
            });

            taskDef.AddContainer("default-container", new ContainerDefinitionProps
            {
                Image = ContainerImage.FromEcrRepository(repository, "latest"),
            });
            
             var ecs = new Cluster(this, "cdk-test-ecs-cluster", new ClusterProps
             {
                ClusterName = "cdk-test-cluster",
                EnableFargateCapacityProviders = true
             });

             var ecsService = new FargateService(this, "cdk-test-fargate-service", new FargateServiceProps
             {
                 Cluster = ecs,
                 TaskDefinition = taskDef,
                 PlatformVersion = FargatePlatformVersion.LATEST,
                 ServiceName = "cdk-test-fargate-service"
             });
        }
    }
}
