# Клиентская часть LiteCall

![Project Image](https://raw.githubusercontent.com/Htomsik/Htomsik/main/Assets/collage.png)


---

## __Оглавление__

- [Описание](#Описание)
- [Возможности приложения](##Что-может-делать-приложение?)
- [Ссылки на авторов](#Ссылки-на-авторов)

---

## __Описание__

Создание  мессенджера LiteCall стало экспериментом из разряда: сможем ли мы за месяц сделать курсовую работу такого уровня?

На данный момент мессенджер реализован на уровне курсовой работы и работает на связке WPF + WCF. Из за недостатка знаний в работе с приложениями которые требуют много асинхронности  + урезании сроков сдачи курсовой работы почти на месяц, огромное количество багов и недоработок.

В дипломной работе WCF будет заменен на SignalR, так как он проще лично для нас работает с потоковой передачей данных.

### __Окно авторизации__

![Project Image](https://raw.githubusercontent.com/Htomsik/LiteCall/master/ReadmeAssets/Login.png)

### __Окно подключения к серверу__

![Project Image](https://raw.githubusercontent.com/Htomsik/LiteCall/master/ReadmeAssets/Main.png)

### __Страница сервера__

![Project Image](https://raw.githubusercontent.com/Htomsik/LiteCall/master/ReadmeAssets/ServerRoom1.png)

### __Диаграмма классов клиента__

![Project Image](https://raw.githubusercontent.com/Htomsik/LiteCall/master/ReadmeAssets/ClassDiagram1.png)



### __Окно сервера__

#### Технологии

- .Net
- WPF
- WCF

#### Паттерн

- mvvm

### nuget пакеты клиента
- NAudio
- System.ServiceModel.Duplex
- System.ServiceModel.Http
- System.ServiceModel.NetTcp
- System.ServiceModel.Primitives
- System.ServiceModel.Security
- System.Windows.Interactivity.WPF

---

## Что может делать приложение?

### Обмениваться текстовыми и аудио сообщениями между пользователями одного сервера


## __Установка__

[Ссылка на последнюю версию установщика клиента]() 
>Установщик временно отсуствует

[Ссылка на установщик сервера]() 
>Установщик временно отсуствует

---
## __Недобавленные возможности/баги:__
- Невозможность подключения больше чем 2 человек в голосовой чат
- Отсутсвие нормального отключения пользователя от сервера
- Отсуствие нормальной валидации
- В силу неопытности неизвестно правильно ли были реализованны отдельные компоненты
- Отсуствие нормального тестирования
---
## __Ссылки на авторов__

Клиентская часть:

[![Костя](https://img.shields.io/badge/-Костя-1C1C22?style=for-the-badge&logo=vk&logoColor=blue)](https://vk.com/jessnjake)


Серверная часть:

[![Артём](https://img.shields.io/badge/-Артём-1C1C22?style=for-the-badge&logo=vk&logoColor=red)](https://vk.com/id506987182)








