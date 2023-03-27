global using Console = Colorful.Console;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Web;
using ImageOSINT2;
using Newtonsoft.Json;
using ScrapySharp.Network;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Color = System.Drawing.Color;
using File = System.IO.File;

const string token = "5470770147:AAF4Hw_N_Hzh9RnVhkIjKk2FWjSNC4RtBE4"; // enter your token here

var admins = new List<long>
{
    938934199
};

var userChatIds = new List<long>();
var urls = new List<string>();
var count = 10;


var bot = new TelegramBotClient(token);
var receiverOptions = new ReceiverOptions();
var updateReceiver = new QueuedUpdateReceiver(bot, receiverOptions);
var browser = new ScrapingBrowser
{
    UserAgent = FakeUserAgents.Chrome,
    KeepAlive = true,
    CachePolicy = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable)
};

try
{
    await foreach (var update in updateReceiver)
    {
        if (update.Message is { } message)
        {
            Log(message, true);
            try
            {
                if (message.Text == "/admin" && admins.Contains(message.From.Id))
                    await bot.SendTextMessageAsync(message.Chat.Id,
                            "Отлично, ты админ.\nДля рассылки пиши /mailing <текст рассылки>\nДля смены количества результатов пиши /count <число>");
                if (message.Text?.StartsWith("/mailing") == true && admins.Contains(message.From.Id))
                    foreach (var user in userChatIds)
                        try
                        {
                            await bot.SendTextMessageAsync(user, string.Join(" ", message.Text.Split(' ').Skip(1)));
                        }
                        catch
                        {
                        }

                if (message.Text?.StartsWith("/count") == true && admins.Contains(message.From.Id))
                    count = int.Parse(message.Text.Split(' ')[1]);
                if (message.Photo != null && message.Photo.Length != 0)
                {
                    var photo = await bot.GetFileAsync(message.Photo.LastOrDefault().FileId);
                    var url = $"https://api.telegram.org/file/bot{token}/" + photo.FilePath;

                    try
                    {
                        urls.Add(url);
                        var index = urls.IndexOf(url);
                        Console.WriteLine($"Url: {url}");
                        Console.WriteLine($"Index: {index}");

                        var keyboard = new InlineKeyboardMarkup(new[]
                        {
                            new[]
                            {
                                new InlineKeyboardButton("Поиск в Яндекс 🔎") { CallbackData = index.ToString() }
                            }
                        });
                        await bot.SendTextMessageAsync(message.Chat.Id, "Выберите вариант поиска",
                            replyMarkup: keyboard);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                if (message.Text == "/start")
                    await bot.SendTextMessageAsync(message.Chat.Id,
                            "Привет, это PhotoSINT by Temnij!\nКидай сюда ссылку на пикчу или саму пикчу)");
                if (message.Text.StartsWith("http"))
                    try
                    {
                        urls.Add(message.Text);
                        var index = urls.IndexOf(message.Text);
                        Console.WriteLine($"Url: {message.Text}");
                        Console.WriteLine($"Index: {index}");

                        var keyboard = new InlineKeyboardMarkup(new[]
                        {
                            new[]
                            {
                                new InlineKeyboardButton("Поиск в Яндекс 🔎") { CallbackData = index.ToString() }
                            }
                        });
                        await bot.SendTextMessageAsync(message.Chat.Id, "Выберите вариант поиска",
                            replyMarkup: keyboard);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
            }
            catch
            {
            }

            Log(message, false);
        }

        if (update.CallbackQuery != null)
            try
            {
                var query = update.CallbackQuery;
                await bot.SendTextMessageAsync(query.Message.Chat.Id, "Подожди...");
                var images = (await GetOtherImagesAsync(urls[int.Parse(query.Data)]))
                    .Take(count);

                foreach (var image in images)
                    try
                    {
                        await bot.SendTextMessageAsync(query.Message.Chat.Id,
                            $"<b>{image.SerpItem.Preview.FirstOrDefault().W}x{image.SerpItem.Preview.FirstOrDefault().H}</b>\n\n" +
                            $"<a href=\"{image.SerpItem.Thumb.Url.Replace("//", "https://")}\">Превьюшка</a>\n" +
                            $"<a href=\"{image.SerpItem.ImgHref}\">Полное изображение, может быть недоступно</a>\n\n" +
                            $"<b>Страница, содержащая изображение:</b> <a href=\"{image.SerpItem.Snippet.Url}\">{WebUtility.HtmlDecode(image.SerpItem.Snippet.Title)}</a>\n\n" +
                            $"<b>Свежесть:</b> <b><i>{image.SerpItem.Freshness}</i></b>\n" +
                            $"<b>Дубликаты:</b>\n{string.Join("\n", image.SerpItem.Dups.Select(x => $"\t\t\t<a href=\"{(x.Url.StartsWith("//") ? "https:" + x.Url : x.Url)}\">{x.W}x{x.H}</a> (Оригинал: <a href=\"{x.Origin?.Url}\">{x.Origin?.W}x{x.Origin?.H}</a>)"))}\n\n" +
                            $"<a href=\"https://yandex.ru{image.SerpItem.DetailUrl}\">Детали</a>\n",
                            parseMode: ParseMode.Html);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}

async Task<List<ImageOne>> GetOtherImagesAsync(string imageurl)
{
    var page = await browser.NavigateToPageAsync(new Uri(
        "https://yandex.ru/images/search?rpt=imageview&from=tabbar&cbir_page=similar&url=" +
        HttpUtility.UrlEncode(imageurl)));
    return page.Html
        .SelectNodes("/html/body/div[3]/div[2]/div/div/div[2]/div[1]/div/div")
        .Select(x => JsonConvert.DeserializeObject<ImageOne>(x.Attributes["data-bem"].Value))
        .ToList()!;
}

void Log(Message message, bool action)
{
    var endText = new StringBuilder();

    endText.Append(action ? "Processing request from: " : "End processing request from: ");

    if (!string.IsNullOrEmpty(message.From.Username))
        endText.Append('@').Append(message.From.Username).Append(' ');

    endText.Append(message.From.FirstName);

    if (!string.IsNullOrEmpty(message.From.LastName))
        endText.Append(' ').Append(message.From.LastName);

    endText.Append(" with id ").Append(message.From.Id);

    var endString = endText.ToString();
    if (action)
        Console.WriteLine(endString, Color.Yellow);
    else
        Console.WriteLine("\t" + endString, Color.Green);

    File.AppendAllText("log.log", endString + Environment.NewLine);
    if (!userChatIds.Contains(message.Chat.Id))
        userChatIds.Add(message.Chat.Id);
}