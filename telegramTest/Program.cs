using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var botClient = new TelegramBotClient("6797439955:AAHA_jPPUvpRdIVdIEt2ZeTPkXketnLEnro");

using CancellationTokenSource cts = new();

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
};

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

// Send cancellation request to stop bot
cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Message is not { } message)
    {
        Thread.Sleep(TimeSpan.FromSeconds(10));
        return;
    }
    // Only process text messages
    if (message.Text is not { } messageText)
    {
        Thread.Sleep(TimeSpan.FromSeconds(10));
        return;
    }

    var chatId = message.Chat.Id;


    // Echo received message text
    Message sentMessage = await botClient.SendTextMessageAsync(
        chatId: chatId,
        text: $"Ваши данные отправлены.\n{message.Chat.FirstName}\n{message.Chat.LastName}\n{message.Chat.Username}\nId - " + chatId,
        cancellationToken: cancellationToken);

    Message sentMessageMy = await botClient.SendTextMessageAsync(
        chatId: 787471566,
        text: $"\nНовое\n{message.Chat.FirstName}\n{message.Chat.LastName}\n{message.Chat.Username}\nId - " + chatId,
        cancellationToken: cancellationToken);

    Thread.Sleep(TimeSpan.FromSeconds(10));
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}
