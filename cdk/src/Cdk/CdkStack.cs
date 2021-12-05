using Amazon.CDK;
using Amazon.CDK.AWS.ApplicationAutoScaling;
using Amazon.CDK.AWS.ECR;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.ECS.Patterns;
using Amazon.CDK.AWS.ElasticLoadBalancingV2;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Logs;
using Amazon.CDK.AWS.SQS;

namespace Cdk
{
    public class CdkStack : Stack
    {
        internal CdkStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            var queue = new Queue(this, "cdk-test-queue");

            var ecsCluster = new Cluster(this, "cdk-test-ecs-cluster", new ClusterProps
            {
                ClusterName = "cdk-test-cluster",
                EnableFargateCapacityProviders = true
            });

            var taskRole = new Role(this, "cdk-test-cluster-role", new RoleProps
            {
                RoleName = "cdk-test-ecs-task-role",
                AssumedBy = new AnyPrincipal(),
                ManagedPolicies = new[]
                {
                    ManagedPolicy.FromManagedPolicyArn(
                        this, 
                        "ecs-access-to-sqs-policy",
                        "arn:aws:iam::aws:policy/AmazonSQSFullAccess")
                }
            });

            ApplicationLoadBalancedFargateService loadBalancedFargateService =
                new ApplicationLoadBalancedFargateService(this, "Service",
                    new ApplicationLoadBalancedFargateServiceProps
                    {
                        Cluster = ecsCluster,
                        MemoryLimitMiB = 4096,
                        DesiredCount = 1,
                        Cpu = 2048,
                        TaskImageOptions = new ApplicationLoadBalancedTaskImageOptions
                        {
                            Image = ContainerImage.FromEcrRepository(
                                Repository.FromRepositoryName(this, "cdk-test", "cdk_test_ecr_repo"), "latest"),
                            TaskRole = taskRole
                        },
                        ServiceName = "cdk-test-ecs-service"
                    });

            ScalableTaskCount scalableTarget = loadBalancedFargateService.Service.AutoScaleTaskCount(
                new EnableScalingProps
                {
                    MinCapacity = 1,
                    MaxCapacity = 3
                });

            scalableTarget.ScaleOnCpuUtilization("CpuScaling", new CpuUtilizationScalingProps
            {
                TargetUtilizationPercent = 50,
            });

            scalableTarget.ScaleOnMemoryUtilization("MemoryScaling", new MemoryUtilizationScalingProps
            {
                TargetUtilizationPercent = 50
            });
        }
    }
}