using BlackLineFilterBot.Commands;
using BlackLineFilterBot.Commands.Data;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace BlackLineFilterBot
{
    public sealed class Bot
    {
        private readonly string _token;
        private readonly List<ICommand> _commands;

        public static TelegramBotClient BotClient { get; private set; }

        public Bot(string token)
        {
            _token = token;
            InitializeBotCommands(out _commands);
            InitializeBotClient();
        }

        private void InitializeBotCommands(out List<ICommand> commands)
        {
            commands = new List<ICommand>
            {
                new Command
                {
                    CommandName = "start",
                    Action = (msg) =>
                    {
                        BotClient.SendTextMessageAsync(msg.Chat.Id, "Send me image, and I will resend version with black lines on it");
                    }
                },
            };
        }

        private void InitializeBotClient()
        {
            BotClient = new TelegramBotClient(_token);
            BotClient.OnMessage += BotClient_OnMessage;
        }
 

        private async void BotClient_OnMessage(object sender, MessageEventArgs e)
        {
            try
            {


                if (e.Message.Type == MessageType.Photo || (e.Message.Type == MessageType.Document && e.Message.Document.MimeType.Contains("image")))
                {
                    var fileId = e.Message.Photo?.Last().FileId ?? e.Message.Document.FileId;
                    var fileName = e.Message.Document?.FileName ?? "image_blacklinefilter.jpg";

                    var file = await BotClient.GetFileAsync(fileId);
                    using (var stream = new MemoryStream())
                    {
                        await BotClient.DownloadFileAsync(file.FilePath, stream);
                        var processedImage = await ImageProcessor.AddBlackLinesAsync(new System.Drawing.Bitmap(stream));
                        using (var sendStream = new MemoryStream())
                        {
                            processedImage.Save(sendStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                            var photoToSend = new InputOnlineFile(sendStream, fileName);
                            sendStream.Position = 0;
                            await BotClient.SendPhotoAsync(e.Message.Chat.Id, sendStream);
                            sendStream.Position = 0;
                            await BotClient.SendDocumentAsync(e.Message.Chat.Id, photoToSend);
                        }
                    }
                }
                else if (IsCommand(e.Message))
                {
                    var command = _commands.FirstOrDefault(x => x.CommandName == GetCommandName(e.Message));
                    if (command is null)
                    {
                        await BotClient.SendTextMessageAsync(e.Message.Chat.Id, "Unknown command");
                        return;
                    }

                    await command.ExecuteAsync(e.Message);
                }
                else
                {
                    await BotClient.SendTextMessageAsync(e.Message.Chat.Id, "Send command or image, please");
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message); 
                throw;
            }
        }

        private bool IsCommand(Message message)
        {
            return !string.IsNullOrWhiteSpace(message.Text)
                   && message.Text.StartsWith("/");
        }

        private string GetCommandName(Message message)
        {
            // Skip slash character that comes first
            return message.Text.Substring(1);
        }

        public void StartPolling()
        {
            BotClient.StartReceiving();
        }

        public void StopPolling()
        {
            BotClient.StopReceiving();
        }
    }
}