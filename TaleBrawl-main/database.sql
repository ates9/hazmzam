SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";

CREATE TABLE `accounts` (
  `Id` bigint(20) NOT NULL,
  `Trophies` int(11) NOT NULL,
  `Data` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL CHECK (json_valid(`Data`))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;


CREATE TABLE `alliances` (
  `Id` bigint(20) NOT NULL,
  `Name` text NOT NULL,
  `Trophies` int(11) NOT NULL,
  `Data` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_bin NOT NULL CHECK (json_valid(`Data`))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;


CREATE TABLE `users` (
  `username` varchar(20) NOT NULL,
  `password` varchar(60) NOT NULL,
  `id` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;


ALTER TABLE `accounts`
  ADD UNIQUE KEY `Id` (`Id`);

ALTER TABLE `alliances`
  ADD UNIQUE KEY `Id` (`Id`);


ALTER TABLE `users`
  ADD UNIQUE KEY `id` (`id`);
COMMIT;