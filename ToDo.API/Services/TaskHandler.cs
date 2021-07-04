using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using ToDo.API.Models;

namespace ToDo.API.Services
{
    public class TaskHandler : ITaskHandler
    {
        private static ConnectionFactory _factory;
        private static IConnection _connection;
        private static IModel _model;

        private const string CompletedQueueName = "task.completed";
        private const string InProgressQueueName = "task.inprogress";

        public TaskHandler()
        {
            CreateQueue();
        }
        
        public void CompleteTask(TaskCompletedDto taskCompleted)
        {
            var json = JsonConvert.SerializeObject(taskCompleted);
            var body = Encoding.UTF8.GetBytes(json);
            _model.BasicPublish("", CompletedQueueName, null, body);
        }

        public void InProgressTask(TaskInProgressDto taskInProgress)
        {
            var json = JsonConvert.SerializeObject(taskInProgress);
            var body = Encoding.UTF8.GetBytes(json);
            _model.BasicPublish("", InProgressQueueName, null, body);
        }
        
        private static void CreateQueue()
        {
            //TODO Externalize the configuration
            _factory = new ConnectionFactory {HostName = "localhost", UserName = "guest", Password = "guest"};
            _connection = _factory.CreateConnection();
            _model = _connection.CreateModel();

            _model.QueueDeclare(CompletedQueueName, true, false, false, null);
        }
        
    }
}