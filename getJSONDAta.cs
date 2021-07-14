using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Storage.Blobs;


namespace jeahune.Function
{
    public static class GetJSONData
    {
        [FunctionName("GetJSONData")]
        public static string Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
        HttpRequest req, ILogger log, ExecutionContext context)
        {
            string connStrA = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody); //다이나믹 데이터는 
            // deserial오브젝트에서가져옴
            string valueA = data.a; //a값을 받는다

            BlobServiceClient clientA = new BlobServiceClient(connStrA);
            //이것을 이용해서 컨테이너 가져오고 데이터도 가져올 수 있게 해준다.
            BlobContainerClient containerA = clientA.GetBlobContainerClient("jeahunecon");
            //컨테이너를 먼저 가져오게 하는 명령어 마지막에 컨테이너 이름입력
            BlobClient blobA = containerA.GetBlobClient(valueA + ".json");
            //JSON데이터등을 불러온다. 마지막에 파일명을 입력한다. JSON파일을 집어넣어준다.

            string responseA = "No Data";
            //데이터가 없으면 No Data

            if(blobA.Exists())
            {
                using (MemoryStream msA = new MemoryStream())
                {
                    blobA.DownloadTo(msA);
                    responseA = System.Text.Encoding.UTF8.GetString(msA.ToArray());
                }
            }
            //블랍을 가져와서 텍스트로 가져와서 리턴받아야 한다. 존재하면 사용할 수 있다.
            //위에 생성된 것이 가운데에 입력이되는 명령어
            //텍스트로 바꿔야 되기때문에 시스템.텍스트 리스폰스 A를 리턴받을 수 있다.

            return responseA;
        }
    }
}
