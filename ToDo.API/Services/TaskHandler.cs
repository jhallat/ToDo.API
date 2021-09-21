using System;
using System.Text;
using Microsoft.Extensions.Configuration;
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

        private readonly string _completedQueueName;
        private readonly string _inProgressQueueName;
        
        public TaskHandler(String completedQueueName,
                           String inProgressQueueName,
                           String hostName,
                           String userName,
                           String password)
        {
            _completedQueueName = completedQueueName;
            _inProgressQueueName = inProgressQueueName;
            
            CreateQueues(hostName, userName, password, _completedQueueName, _inProgressQueueName);
        }
        
        public void CompleteTask(TaskCompletedDto taskCompleted)
        {
            var json = JsonConvert.SerializeObject(taskCompleted);
            var body = Encoding.UTF8.GetBytes(json);
            _model.BasicPublish("", _completedQueueName, null, body);
        }

        public void InProgressTask(TaskInProgressDto taskInProgress)
        {
            var json = JsonConvert.SerializeObject(taskInProgress);
            var body = Encoding.UTF8.GetBytes(json);
            _model.BasicPublish("", _inProgressQueueName, null, body);
        }
        
        private static void CreateQueues(string hostName, 
                                         string userName,
                                         string password,
                                         string completedQueue,
                                         string inprogressQueue)
        {
            //_factory = new ConnectionFactory {HostName = "localhost", UserName = "guest", Password = "guest"};
            _factory = new ConnectionFactory {HostName = hostName, UserName = userName, Password = password, VirtualHost = "/"};
            _connection = _factory.CreateConnection();
            _model = _connection.CreateModel();

            _model.QueueDeclare(completedQueue, true, false, false, null);
            _model.QueueDeclare(inprogressQueue, true, false, false, null);
        }
        
    }
}