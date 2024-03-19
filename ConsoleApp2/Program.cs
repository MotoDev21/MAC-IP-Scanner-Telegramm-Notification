using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

class Program
{
    
    static async Task SendBot()
    {
        string botToken = ""; // Замените на токен вашего бота
        long chatId = ; // Замените на chat_id вашей группы

        string messageText = "ТЕТЕРЕВ ОБНАРУЖЕН НА ТЕРРИТОРИИ!!! ПТИЧКА В КЛЕТКЕ!!"; // Текст сообщения

        using (HttpClient httpClient = new HttpClient())
        {
            string apiUrl = $"https://api.telegram.org/bot{botToken}/sendMessage";

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("chat_id", chatId.ToString()),
                new KeyValuePair<string, string>("text", messageText)
            });

            HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Сообщение успешно отправлено.");
            }
            else
            {
                Console.WriteLine($"Ошибка при отправке сообщения: {response.ReasonPhrase}");
            }
        }
    }

    private static ITelegramBotClient botClient;

    static async Task Main()
    {
        Console.Title = "TeterevScaner V1.1 by mmoottoo21";
        Console.SetWindowSize(60,20);
        string baseIpAddressOne = "172.17.20."; // Замените на базовый IP-адрес вашей локальной сети
        string baseIpAddressTwo = "172.17.10."; // Замените на базовый IP-адрес вашей локальной сети
        int startRange = 1; // Начальный диапазон IP-адресов
        int endRange = 254; // Конечный диапазон IP-адресов

        while (true)
        {

            for (int i = startRange; i <= endRange; i++)
            {
                string ipAddress = baseIpAddressTwo + i;
                await ScanIpAddress(ipAddress);
            }

            for (int i = startRange; i <= endRange; i++)
            {
                string ipAddress = baseIpAddressOne + i;
                await ScanIpAddress(ipAddress);
            }
        }
    }

    public static string ConvertIpToMAC(IPAddress ip)
    {
        byte[] addr = new byte[6];
        int length = addr.Length;

        // TODO: Проверить, что результат - NO_ERROR
        SendARP(ip.GetHashCode(), 0, addr, ref length);

        return BitConverter.ToString(addr, 0, 6);
    }


    [DllImport("iphlpapi.dll", ExactSpelling = true)]
    public static extern int SendARP(int DestinationIP, int SourceIP, [Out] byte[] pMacAddr, ref int PhyAddrLen);

    static string CheckPossition = "OUT";

    static async Task ScanIpAddress(string ipAddress)
    {
        Ping ping = new Ping();
        try
        {
            PingReply reply = await ping.SendPingAsync(ipAddress, 100); // Ожидание ответа в течение 1 секунды
            if (reply.Status == IPStatus.Success)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                IPAddress MAC = IPAddress.Parse(ipAddress);
                string MacString = ConvertIpToMAC(MAC).ToString();
                Console.WriteLine($"IP-адрес {ipAddress} доступен." + " MAC: " + MacString);
                Console.ResetColor();
                if (MacString == "D0-88-0C-78-5E-A5") //D0-88-0C-78-5E-A5
                {
                    SendBot();
                    CheckPossition = "IN";
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"ALARM!!");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"IP-адрес {ipAddress} не доступен.");
                Console.ResetColor();
            }
        }
        catch (PingException)
        {
            Console.WriteLine($"IP-адрес {ipAddress} не доступен.");
        }
    }
}
