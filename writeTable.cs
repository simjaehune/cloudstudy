using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos.Table;


namespace jeahune.Function
{
    public static class WriteTable
    {
        [FunctionName("WriteTable")]
        public static void Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
        HttpRequest req, ILogger log, ExecutionContext context)
        {
            string connStrA = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody); //다이나믹 데이터는 
            // deserial오브젝트에서가져옴
            string PartitionKeyA = data.PartitionKey; //파티션키 값을 받아온다.
            string RowKeyA = data.RowKey; //로우키 값을 받아온다.
            string contentA = data.content; //내용값을 받아온다.

            CloudStorageAccount stoA = CloudStorageAccount.Parse(connStrA);
            //이것을 이용할 때 필요한게 마소 에져코스모스 테이블이다.
            CloudTableClient tbC = stoA.CreateCloudTableClient();
            //테이블 클라이언트 가져온다.
            CloudTable tableA = tbC.GetTableReference("tableA");
            //이때 우리가 테이블을 생성해야 한다. 클라우드에 tableA라고 생성을 해놨다.테이블 a가 가져와진다.
            //다음에 데이터를 집어넣는다. 테이블a 집어넣는 명령어

            writeToTable(tableA, contentA, PartitionKeyA, RowKeyA);
        }

        static void writeToTable(CloudTable tableA, string contentA, string PartitionKeyA, string RowKeyA)
        //함수를 아래쪽에 생성한다 위에 생성하는 것은 기니까.
        {
            MemoData memoA = new MemoData();
            //밑에 있는 클래스를 객체로 생성한다.
            memoA.PartitionKey = PartitionKeyA;
            memoA.RowKey = RowKeyA;
            //파티션키와 로우키가 반드시 입력이 되어야 한다.
            memoA.content = contentA;

            TableOperation operA = TableOperation.InsertOrReplace(memoA);
            tableA.Execute(operA);
        }

        private class MemoData: TableEntity
        {
            public string content { get; set; }
        }
    }
}
