using NPOI.SS.Formula.Functions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace RebbitMQproject
{   /*
    изменить считывание из бд картинки
    */
    class Program
    {
        public static TestDBContext _context = new TestDBContext();

        public static void Main(string[] args)
        {
            RPC_Sample();
        }

        private static void RPC_Sample()
        {
            RPC_Server();
        }


        public static void RPC_Server()
        {
            var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672, UserName = "root", Password = "root" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "testing_quer", durable: false,
                  exclusive: false, autoDelete: false, arguments: null);
                channel.BasicQos(0, 1, false);
                var consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume(queue: "testing_quer",
                  autoAck: false, consumer: consumer);
                Console.WriteLine(" [x] Awaiting RPC requests");

                consumer.Received += (eventModel, ea) =>
                {
                    Image image= null;
                    var UserId = Encoding.UTF8.GetString(ea.Body);
                    var ImageByte = _context.AspNetUsers.FirstOrDefaultAsync(e=>e.Id==UserId).Result.Image;
                    var props = ea.BasicProperties;
                    var replyProps = channel.CreateBasicProperties();
                    replyProps.CorrelationId = props.CorrelationId;
                    try
                    {
                        image= CropImage(100, new MemoryStream(ImageByte)); /////////////////
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(" [.] " + e.Message);
                    }
                    finally
                    {
                        channel.BasicPublish(exchange: "", routingKey: props.ReplyTo,
                            basicProperties: replyProps, body: ImageToByteArray(image));
                        channel.BasicAck(deliveryTag: ea.DeliveryTag,
                            multiple: false);
                    }

                };

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
        static byte[] ImageToByteArray(Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        static Image CropImage(int newSize, Stream filename)
        {
            Image src = Image.FromStream(filename);
            if (src.Width <= newSize)
                newSize = src.Width;

            var newHeight = src.Height * newSize / src.Width;

            if (newHeight > newSize)
            {
                // Resize with height instead
                newSize = src.Width * newSize / src.Height;
                newHeight = newSize;
            }

            return src.GetThumbnailImage(newSize, newHeight, null, IntPtr.Zero);
        }
    }
}
