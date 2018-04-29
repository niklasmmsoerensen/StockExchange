For at køre RabbitMQ i en container gøres følgende:
	1. Åben Docker Quickstart Terminal (https://download.docker.com/win/stable/DockerToolbox.exe)
	2. Kør: docker run -d --hostname my-rabbit --name rabbitMQ -p 5672:5672 -p 15672:15672 rabbitmq:3-management
	3. Kør: docker-machine ls
	4. Der burde være en enkel maskine med IP 192.168.99.100
	5. RabbitMQ kører nu på 192.168.99.100:5672 og management plugin kører på 192.168.99.100:15672 (user og pass = "guest")
