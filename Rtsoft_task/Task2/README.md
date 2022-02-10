## Задание.

2. Реализовать программу клиент и сервер для работы с TCP
2.1 Сервер принимает json команду на запуск/останов процессе с заданным именем
2.2 Реализовать запуск останов сервиса c переданным именем при помощи команд DBus по переданной команде
  https://unixforum.org/viewtopic.php?t=146380
  
3. В приложение сервера
3.1 Читать
- Чтение температуры процессора из sysfs (/sys/devices/virtual/thermal/thermal_zoneX/temp)
- Загрузку CPU от запущенного сервиса
3.2 Публиковать при помощи MQTT и паковать в protobuf

4. В приложение клиента
4.1 На клиенте отобразить график температуры и график загрузки процессора каждым из запущенных сервисов

## Используемые технологии и инструменты
1. Язык С# (.net core 3.1)
2. Avalonia UI -  .NET фреймворк для разработки кроссплатформенного UI
3. Json.NET - Newtonsoft
4. Autofac - Ioc container
5. XUnit - unit test framework
6. Qdbusviewer - графический D-Bus браузер
7. Vim - свободный текстовый редактор, созданный на основе более старого vi
8. Linux bash для создания и развертывания сервиса

## Демонстрация работы
![](images/demo.gif)


## UML
Диаграмма последовательности взаимодействия клиента и сервера
![PlantUML](http://www.plantuml.com/plantuml/png/hP91QiCm44NtEiLS87HVWWcKNg3PUYDBaqH47XbfvFPEFK9tELbh7DTkXXW5kX8M_u_dVVrTOXqvjrvrFwSphjF4yE8T4l-vscFiCIgb3RjUCquNq_Va6G_OL5V1CxmcrertwGGc4A5XfKnqx8psaB8ncD8XUp0weHezcshoHZown1Y1y1rombYJ9WicecOSBclYKqHjuH0Yd5pxEs7SO_5tHqS8PuK-8HoqysYdVLoWTTieLqXMgIlWh9ntWOXjOAofYs6W0jRfB08A7Gji6TgsgIwVRvdWEklojNrgAZ2j5HavwiCPLnm2xhxqDY7yG9CPnOI5UR0j4oe479VOS-YA4OLbZL5g2o3BHoo0BTu-SK59e2RRv0G9O-yC_ajLXe658Kgvf9Uw1njoTyJZRPcVeXnIwDTxKiPPufUGiY8x99jR_mq0)