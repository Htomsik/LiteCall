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
    <img src="https://raw.githubusercontent.com/Htomsik/Htomsik/main/Assets/LiteCall/LiteCallAppIcon.png" alt="Logo" width="300">
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

* [![Naudio][NAudio-shield]][NAudio-url]
* [![ReactiveUI][ReactiveUI-shield]][ReactiveUI-url]
* [![Newtonsoft][Newtonsoft-shield]][Newtonsoft-url]
* [![SignalR][SignalR-shield]][SignalR-url]




<p align="right">(<a href="#top">back to top</a>)</p>


<!-- ROADMAP -->
# Roadmap

  ## Взаимодействия с сервером для общения

  - [ ] Различные варианты подключения к серверам для общения:
    - [x] По Ip адрессу API сервера чата
    - [x] По названию сервера чата в главной базе 
    - [ ] Выбор из списка общедоступных серверов в специальном окне
  
    ### Управление комнатами
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
      
    ### Взаимодействия в комнате
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

    ### Администрирование комнаты
      - [ ] Взаимодействие с пользователями
        - [ ] Кик из комнаты
          - [ ] с возможностью перезахода
          - [ ] Бан
            - [ ] Временный
            - [ ] Перманентный (только в этой комнате)
        - [ ] Мут
          - [ ] Отключения микрофона пользователя для всех в комнате (только в этой комнате)
          - [ ] Отключения наушников и микрофона для пользователя (только в этой комнате)

      ### Администрирование сервера
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

      ### Управление аккаунтом
      - [ ] Добавление сервера и аккаунта в закладки
      - [ ] Управление аккаунтом
        - [x] Регистрция
        - [x] Авторизация
          - [ ] Двойная аутинтификация через генератор кодов
        - [x] Восстановление пароля
          - [x] По секретному вопросу
        - [ ] Смена никнейма (в случае если аккаунт не ялвяется зарегистрированным)
    

## Взаимодействия с главным сервером
### Управление аккаунтом  
- [x] Регистрция
  - [x] Авторизация
  - [ ] Двойная аутинтификация через генератор кодов/почту
  - [x] Восстановление пароля 
    - [x] По секретному вопросу
    - [ ] По почте
  - [ ] Смена никнейма (в случае если аккаунт не ялвяется зарегистрированным)

### Хранение данных
- [x] Синхронизация списка сохраненных серверов и аккаунтов к ним
- [ ] Синхронизация списка друзей
- [ ] Синхронизция настроек клиента

## Настройки клиента
### Общее
- [ ] Поддержка разных языков
  - [x] Английский
  - [ ] Русский
- [ ] Управление списком друзей
- [x] Управление списком сохраненных серверов и аккаунтов

### Аудио
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
    







<p align="right">(<a href="#top">back to top</a>)</p>



<!-- CONTACT -->
# Contact

[![Konstantin](https://img.shields.io/badge/Konstantin-CLient-090909?style=for-the-badge&logo=vk&logoColor=red)](https://vk.com/jessnjake)

[![Artem](https://img.shields.io/badge/Artem-Servers-090909?style=for-the-badge&logo=vk&logoColor=blue)](https://vk.com/id506987182)

[![YouTube](https://img.shields.io/badge/LiteCall-YouTube-090909?style=for-the-badge&logo=YouTube&logoColor=FF0000)](https://www.youtube.com/watch?v=dQw4w9WgXcQ)

<details>
  <summary>Also Creator</summary>
<h1 align="center">Kifirka</h1>
<img src="https://raw.githubusercontent.com/Htomsik/Htomsik/main/Assets/ProileReadme/Chifirka.png">
</details>

<p align="right">(<a href="#top">back to top</a>)</p>

<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
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


