## Описание клиентской части.

Клиентская часть - десктоп приложение, включающее следующие
элементы управления:

![task2](pictures/client_control_panel.png?raw=true "Панель управления клиента")

Описание элементов управления:
1. Поле ввода ip адреса
2. Номер порта
3. Имя пользователя
4. Кнопка подключения к удаленному серверу
5. Информационное окно сообщений
6. Имя процесса для запуска/останова
7. Аргументы команды (при необходимости запуска процесса с аргументами.)
8. Кнопка запуска процесса
9. Кнопка останова процесса
10. Имя удаленного сервиса для запуска/останова
11. Пароль для рутовой записи (sudo)
12. Кнопка запуска сервиса
13. Кнопка останова сервиса


## Скриншоты по третьей части задания

Верхний график - график температуры CPU (из sysfs (/sys/devices/virtual/thermal/thermal_zoneX/temp) )
Нижний график - график загрузки процессом CPU(Main PID)
![task3](pictures/client_control_panel_with_cpu_data.png?raw=true "Панель управления клиента")

Код программы, запущенной сервисами foo-daemon.service и foo-daemon1.service
![task3](pictures/simple_programm.png?raw=true "Simple Program")

Мониторинг загрузки сервисами foo-daemon.service и foo-daemon1.service CPU
![task3](pictures/service_data.png?raw=true "CPU Loading by foo-daemon.service and foo-daemon1.service")