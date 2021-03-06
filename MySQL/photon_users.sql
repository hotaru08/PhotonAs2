-- MySQL dump 10.13  Distrib 8.0.11, for Win64 (x86_64)
--
-- Host: localhost    Database: photon
-- ------------------------------------------------------
-- Server version	8.0.11

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
 SET NAMES utf8 ;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
 SET character_set_client = utf8mb4 ;
CREATE TABLE `users` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `name` varchar(45) NOT NULL,
  `password` varchar(45) DEFAULT NULL,
  `date_created` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=36 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (7,'Kry','','2018-06-02 19:00:45'),(8,'jolyn','123','2018-06-02 20:11:06'),(10,'jolyn1','','2018-06-03 14:18:37'),(11,'jolyn2','','2018-06-04 03:08:42'),(12,'jolyn23','','2018-06-04 03:09:36'),(17,'Guest255','123','2018-06-04 05:21:42'),(18,'Guest1192','123','2018-06-04 05:22:14'),(19,'Guest8828','123','2018-06-04 05:24:57'),(20,'Guest1084','123','2018-06-04 05:27:04'),(21,'Guest2746','123','2018-06-04 05:34:05'),(22,'Guest969','123','2018-06-04 05:34:14'),(23,'Guest1647','123','2018-06-04 05:34:39'),(24,'Guest1128','123','2018-06-04 05:35:29'),(25,'Guest6677','123','2018-06-04 05:35:47'),(26,'Guest5042','123','2018-06-04 05:43:47'),(27,'Guest8986','123','2018-06-04 05:46:34'),(28,'Guest23','123','2018-06-04 05:48:46'),(29,'sup','','2018-06-04 06:34:53'),(30,'hello123','','2018-06-04 09:31:46'),(31,'JolynWong','','2018-06-04 10:30:48'),(32,'reeeeeee','','2018-06-04 10:38:46'),(33,'Kry2','','2018-06-05 12:54:01'),(34,'Kry3','','2018-06-05 13:14:10'),(35,'hello1234','','2018-06-05 17:32:08');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2018-06-06  1:10:03
