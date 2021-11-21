using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace CdkTest.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IAmazonSQS _amazonSqs;

    public IndexModel(ILogger<IndexModel> logger, IAmazonSQS amazonSqs)
    {
        _logger = logger;
        _amazonSqs = amazonSqs;
    }

    public void OnGet()
    {
    }

    public async Task OnPost()
    {
        await _amazonSqs.SendMessageAsync(new SendMessageRequest
        {
            QueueUrl =
                "https://sqs.eu-west-1.amazonaws.com/258166098025/CdkTestStack-cdktestqueueDBFB05DC-16T19OBVW5TO1",
            MessageBody = "test_msg"
        });
    }
}