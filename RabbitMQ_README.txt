For at k�re RabbitMQ i en container g�res f�lgende:
	1. �ben Docker Quickstart Terminal (https://download.docker.com/win/stable/DockerToolbox.exe)
	2. K�r: docker run -d --hostname my-rabbit --name rabbitMQ -p 5672:5672 -p 15672:15672 rabbitmq:3-management
	3. K�r: docker-machine ls
	4. Der burde v�re en enkel maskine med IP 192.168.99.100
	5. RabbitMQ k�rer nu p� 192.168.99.100:5672 og management plugin k�rer p� 192.168.99.100:15672 (user og pass = "guest")
