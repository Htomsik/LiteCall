<div id="top"></div>

<!-- PROJECT SHIELDS -->
[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]

<!-- PROJECT LOGO -->
<br />
<div align="center">
  <a>
    <img src="https://raw.githubusercontent.com/Htomsik/Htomsik/main/Assets/LiteCall/LiteCallAppIcon.png" alt="Logo" width="200">
  </a>

<h3 align="center">LiteCall</h3>

  <p align="center">
    Can you hear me?
    <br />
    <a href="https://github.com/Htomsik/LiteCall/issues">Report Bug</a>
    ·
    <a href="https://github.com/Htomsik/LiteCall/issues">Request Feature</a>
  </p>
</div>


<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#Screenshots">Screenshots</a></li>
    <li><a href="#Also-creator">Also-creator</a></li>
    <li><a href="#contact">Contact</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
# About The Project

![Product Name Screen Shot](https://raw.githubusercontent.com/Htomsik/Htomsik/main/Assets/LiteCall/Main.png)

LiteCall is a voip client and have only one task - create a voip with the functionality i and my friends need.

We started to create it with my [friend](https://github.com/Code-Nostra) as a thesis, but later it grew into an independent project.

<p align="right">(<a href="#top">back to top</a>)</p>


## Built With

### Client
[![.Net][.Net-shield]][.Net-url]
[![SignalR][SignalR-shield]][SignalR-url]
[![ReactiveUI][ReactiveUI-shield]][ReactiveUI-url]
[![Naudio][NAudio-shield]][NAudio-url]
[![Newtonsoft][Newtonsoft-shield]][Newtonsoft-url]

### Authentication server (also main server)
[![Asp.Net][Asp.Net-shield]][Asp.Net-url]
[![SqlLite.Net][SqlLite-shield]][SqlLite-url]
[![EntityFramework.Net][EntityFramework-shield]][EntityFramework-url]

### Chat-server
[![Asp.Net][Asp.Net-shield]][Asp.Net-url]
[![SignalR][SignalR-shield]][SignalR-url]
<p align="right">(<a href="#top">back to top</a>)</p>


<!-- ROADMAP -->
# Roadmap

### Interacting with Chat-server

- [ ] Various options for connecting to Chat-server:
    - [x] By Ip address of the Chat-server API
    - [x] By name of the Chat-server in the main server database
    - [ ] Choice from list of public servers in special window

  #### Room Management
    - [ ] Creation
        - [x] Public
            - [x] without password
            - [x] with password
        - [ ] Private (invisible)
            - [ ] For administration (no one can see them and administrator connects users from other rooms)
            - [ ] For specific roles
                - [ ] without password
                - [ ] with password
            - [ ] Only token connection (visible only to those who are in room)
    - [ ] Editing
        - [ ] Room information
            - [ ] title
            - [ ] password
            - [ ] maximum users in room
  #### Room Interactions
    - [ ] Chat
        - [x] Voice
            - [x] Mute microphone
            - [x] Mute output device (microphone/speakers or something else)
        - [x] Text
            - [x] Text
            - [ ] Images
            - [ ] links
    - [ ] User interaction
        - [ ] Adding to friends list
        - [ ] Changing incoming audio volume of user (on client, only for yourself)
        - [ ] Muting a user (on client, only for yourself)

  #### Room Administration
    - [ ] User interaction
        - [ ] Kick from the room
            - [ ] With ability to reconnecting
            - [ ] Ban
                - [ ] Temporary
                - [ ] Permanent (only in this room)
        - [ ] Mute
            - [ ] Deactivate user microphone for everyone in the room (only in this room)
            - [ ] Deactivate headphones and microphone for the user (only in this room)

  #### Server Administration
    - [ ] User interaction
        - [x] Kick from the room
            - [x] With ability to reconnecting
            - [ ] Ban
                - [ ] Temporary (in all rooms)
                - [ ] Permanent (in all rooms)
        - [ ] Ban on server
            - [ ] Temporary
            - [ ] Permanent
        - [ ] Mute on the server
            - [ ] Temporary
            - [ ] Permanent
    - [ ] Change of server settings
        - [ ] Descriptions
        - [ ] Chat server IP address
        - [ ] Maximum number of users

  #### Account Management
    - [ ] Adding server and account to bookmarks
    - [ ] Account management
        - [x] Sign up
        - [x] Sign in
            - [ ] Double authentication via code generator
        - [x] Password recovery
            - [x] By secret question
        - [ ] Nickname change (if your account is not registered )


### Interacting with the main server
#### Account Management
- [x] Registration
    - [x] Authentication
    - [ ] Double Authentication via code generator/email
    - [x] Password recovery
        - [x] By secret question
        - [ ] By mail
    - [ ] Nickname change (if your account is not registered )

#### Storing data
- [x] Synchronization list of saved servers and accounts of this servers
- [ ] Synchronization of friends list
- [ ] Synchronization client settings

### Client settings
#### General
- [ ] Different language support
    - [x] English
    - [ ] Russian
- [ ] Manage your Friends list
- [x] Manage your list of saved servers and accounts of this servers

#### Audio
- [ ] Audio Input device selection
- [ ] Audio output device selection
- [ ] Mute microphone when mute output device
- [ ] Noise reduction
    - [x] Standard (VAD filter)
    - [ ] Moved (noise reduction without VAD)
    - [ ] Hybrid (Standard + Advanced)
    - [ ] Based on machine learning (Worth a try)
- [ ] Selecting activation option to send sound
    - [ ] By voice
    - [ ] By button
    - [ ] Permanent recording



    
<details>
  <summary>Russian version</summary>

### Взаимодействия с сервером для общения

- [ ] Различные варианты подключения к серверам для общения:
    - [x] По Ip адрессу API сервера чата
    - [x] По названию сервера чата в главной базе
    - [ ] Выбор из списка общедоступных серверов в специальном окне

  #### Управление комнатами
    - [ ] Создание
        - [x] Публичные
            - [x] без пароля
            - [x] с паролем
        - [ ] Приватные (невидимые)
            - [ ] Для администрации (их невидит никто, а подключение происходит посредством перекидывания пользователей из других комнат )
            - [ ] Для определенных ролей
                - [ ] без пароля
                - [ ] с паролем
            - [ ] Only token connection (Видимые только для тех кто находится в них)
    - [ ] Редактирование
        - [ ] Редактирование информации о комнате
            - [ ] названия
            - [ ] пароля
            - [ ] максимального количества пользователей
        - [ ] Перевод комнаты в Only token connection

  #### Взаимодействия в комнате
    - [ ] Чат
        - [x] Голосовой
            - [x] Выключение микрофона
            - [x] Выключение звука
        - [x] Текстовый
            - [x] Текст
            - [ ] Изображения
            - [ ] Ссылки
    - [ ] Взаимодействие с пользователями
        - [ ] Добваление в список друзей
        - [ ] Изменение громкости входящего аудио пользователей (на клиенте)
        - [ ] Мут пользователя (на клиенте, только для себя)

  #### Администрирование комнаты
    - [ ] Взаимодействие с пользователями
        - [ ] Кик из комнаты
            - [ ] с возможностью перезахода
            - [ ] Бан
                - [ ] Временный
                - [ ] Перманентный (только в этой комнате)
        - [ ] Мут
            - [ ] Отключения микрофона пользователя для всех в комнате (только в этой комнате)
            - [ ] Отключения наушников и микрофона для пользователя (только в этой комнате)

  #### Администрирование сервера
    - [ ] Взаимодействие с пользователями
        - [x] Кик из комнаты
            - [x] С возможностью перезахода
            - [ ] Бан
                - [ ] Временный (во всех комнатах)
                - [ ] Перманентный (во всех комнатах)
        - [ ] Бан на сервере
            - [ ] Временный
            - [ ] Перманентный
        - [ ] Мут на сервере
            - [ ] Временный
            - [ ] Перманентный
    - [ ] Изменение настроек сервера
        - [ ] описания
        - [ ] Ip адреса чат сервера
        - [ ] Максимального количества пользователей

  #### Управление аккаунтом
    - [ ] Добавление сервера и аккаунта в закладки
    - [ ] Управление аккаунтом
        - [x] Регистрция
        - [x] Авторизация
            - [ ] Двойная аутинтификация через генератор кодов
        - [x] Восстановление пароля
            - [x] По секретному вопросу
        - [ ] Смена никнейма (в случае если аккаунт не ялвяется зарегистрированным)


### Взаимодействия с главным сервером
#### Управление аккаунтом
- [x] Регистрция
    - [x] Авторизация
    - [ ] Двойная аутинтификация через генератор кодов/почту
    - [x] Восстановление пароля
        - [x] По секретному вопросу
        - [ ] По почте
    - [ ] Смена никнейма (в случае если аккаунт не ялвяется зарегистрированным)

#### Хранение данных
- [x] Синхронизация списка сохраненных серверов и аккаунтов к ним
- [ ] Синхронизация списка друзей
- [ ] Синхронизция настроек клиента

### Настройки клиента
#### Общее
- [ ] Поддержка разных языков
    - [x] Английский
    - [ ] Русский
- [ ] Управление списком друзей
- [x] Управление списком сохраненных серверов и аккаунтов

#### Аудио
- [ ] Выбор устройства ввода аудио
- [ ] Выбор устройства вывода аудио
- [ ] Возможность выключение микрофона при выключении звука
- [ ] Шумоподавление
    - [x] Стандартное (VAD фильтр)
    - [ ] Подвинутое (снижение уровня шума без VAD)
    - [ ] Гибридное (Стандатное + продвинутое)
    - [ ] На основе машинного обучения (Стоит попробовать)
- [ ] Выбор варианта активации отправки звука
    - [ ] По голосу
    - [ ] По кнопке
    - [ ] Постояння запись


</details>

<p align="right">(<a href="#top">back to top</a>)</p>

# Screenshots
<details>
  <summary>Open me!</summary>
<h3 align="center">Chat Server</h3>
<img src="https://raw.githubusercontent.com/Htomsik/Htomsik/main/Assets/LiteCall/ChatServer.png">
<h3 align="center">Settings</h3>
<img src="https://raw.githubusercontent.com/Htomsik/Htomsik/main/Assets/LiteCall/Settings.png">
<h3 align="center">Main page</h3>
<img src="https://raw.githubusercontent.com/Htomsik/Htomsik/main/Assets/LiteCall/Main.png">
</details>
<p align="right">(<a href="#top">back to top</a>)</p>

# Also creator
<details>
  <summary>Open me!</summary>
<h1 align="center">Kifirka</h1>
<img src="https://raw.githubusercontent.com/Htomsik/Htomsik/main/Assets/ProileReadme/Chifirka.png">
</details>
<p align="right">(<a href="#top">back to top</a>)</p>

# Contact

[![Konstantin](https://img.shields.io/badge/Konstantin-CLient-090909?style=for-the-badge&logo=vk&logoColor=red)](https://vk.com/jessnjake)
[![Artem](https://img.shields.io/badge/Artem-Servers-090909?style=for-the-badge&logo=vk&logoColor=blue)](https://vk.com/id506987182)
[![YouTube](https://img.shields.io/badge/LiteCall-YouTube-090909?style=for-the-badge&logo=YouTube&logoColor=FF0000)](https://www.youtube.com/watch?v=dQw4w9WgXcQ)

<p align="right">(<a href="#top">back to top</a>)</p>

<!-- MARKDOWN LINKS & IMAGES -->
[contributors-shield]: https://img.shields.io/github/contributors/Htomsik/LiteCall.svg?style=for-the-badge
[contributors-url]: https://github.com/Htomsik/LiteCall/graphs/contributors

[forks-shield]: https://img.shields.io/github/forks/Htomsik/LiteCall.svg?style=for-the-badge
[forks-url]: https://github.com/Htomsik/LiteCall/network/members

[stars-shield]: https://img.shields.io/github/stars/Htomsik/LiteCall.svg?style=for-the-badge
[stars-url]: https://github.com/Htomsik/LiteCall/stargazers

[issues-shield]: https://img.shields.io/github/issues/Htomsik/LiteCall.svg?style=for-the-badge
[issues-url]: https://github.com/othneildrew/Htomsik/LiteCall

[license-shield]: https://img.shields.io/github/license/Htomsik/LiteCall.svg?style=for-the-badge
[license-url]: https://github.com/Htomsik/LiteCall/blob/master/LICENSE.txt

[NAudio-shield]: https://img.shields.io/badge/NAudio-090909?style=for-the-badge&logo=
[NAudio-url]: https://github.com/naudio/NAudio

[ReactiveUI-shield]: https://img.shields.io/badge/ReacctiveUI-090909?style=for-the-badge&logo=
[ReactiveUI-url]: https://www.reactiveui.net/

[Newtonsoft-shield]: https://img.shields.io/badge/Json.NET-090909?style=for-the-badge&logo=
[Newtonsoft-url]: https://www.newtonsoft.com/json

[SignalR-shield]: https://img.shields.io/badge/SignalR-090909?style=for-the-badge&logo=
[SignalR-url]: https://docs.microsoft.com/ru-ru/aspnet/core/tutorials/signalr?view=aspnetcore-6.0&tabs=visual-studio

[.Net-shield]: https://img.shields.io/badge/.Net-090909?style=for-the-badge&logo=
[.Net-url]: https://dotnet.microsoft.com/en-us/

[Asp.Net-shield]: https://img.shields.io/badge/Asp.Net-090909?style=for-the-badge&logo=
[Asp.Net-url]: https://dotnet.microsoft.com/en-us/apps/aspnet

[EntityFramework-shield]: https://img.shields.io/badge/EntityFramework-090909?style=for-the-badge&logo=
[EntityFramework-url]: https://docs.microsoft.com/ru-ru/ef/

[SqlLite-shield]: https://img.shields.io/badge/SqlLite-090909?style=for-the-badge&logo=
[SqlLite-url]: https://www.sqlite.org/index.html
