using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using SignalRServ;

namespace SignlR
{
    internal class Program
    {
        public static string url = "http://localhost:5000/LiteCall";

        static async Task Main(string[] args)
        {

            InitSignalRConnection();
            start:
            //var a=ClientMethos.hubConnection.SendAsync("GetRoomList");
            Console.WriteLine("Введите имя группы? ");
            string group = Console.ReadLine();
            Console.WriteLine("Подключиться(1) |Создать?(0)");
            string temp = Console.ReadLine();
            if (temp == "0")
            {
                await ClientMethos.hubConnection.InvokeAsync("GroupCreate", group);

                //await ClientMethos.hubConnection.InvokeAsync("GroupDisconnect");
            }
            else
                await ClientMethos.hubConnection.InvokeAsync("GroupConnect", group);
            //await ClientMethos.hubConnection.InvokeAsync("GroupCreate", group);

            bool isExit = false;

            while (!isExit)
            {
                start2:
                Console.WriteLine("Enter your message or command");
                var userInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(userInput)) continue;

                if (userInput == "exit") isExit = true;
                else if (userInput == "setname")
                {
                    string name = Console.ReadLine();
                    //if (string.IsNullOrWhiteSpace(name)) url += $"?name={name}";
                    var flag = await ClientMethos.hubConnection.InvokeAsync<bool>("SetName", name);
                    if (!flag) { Console.WriteLine("Имя занято"); goto start2; }

                    else
                        Console.WriteLine("Name saved");
                    Console.WriteLine("Войти в другую комнату? 1-да");
                    if (Console.ReadLine() == "1")
                    {
                        string group2 = Console.ReadLine();
                        await ClientMethos.hubConnection.InvokeAsync("GroupDisconnect");
                        goto start;
                    }
                    Console.WriteLine("Запросить список групп и пользователей? 1-да");
                    if (Console.ReadLine() == "1")
                    {
                        List<string> ab = ClientMethos.hubConnection.InvokeAsync<List<string>>("GetRooms").Result;

                        foreach (string a in ab)
                            Console.WriteLine(a);
                        List<string> ab2 = await ClientMethos.hubConnection.InvokeAsync<List<string>>("GetUsersRoom", group);
                        foreach (string a in ab2)
                            Console.WriteLine(a);
                    }
                }
                else
                {
                    var message = new Message { text = userInput };
                    await ClientMethos.hubConnection.SendAsync("SendMessage", message); //вызываем метод концентратора на сервере используя
                                                                                        //указанное имя метода и аргументы
                                                                                        //Вызываем метод сервера
                }
            }
            //await ClientMethos.hubConnection.SendAsync("GroupDisconect", g);
            goto start;

        }
        private static void InitSignalRConnection()
        {
            ClientMethos.ConnectionHub(url);
        }

        //static void Show(Message msg) => Console.WriteLine(msg.text+"Лошара");
    }
}
