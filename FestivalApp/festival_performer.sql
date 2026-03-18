CREATE TABLE person (
  person_id     INT(10)  NOT NULL AUTO_INCREMENT,
  name          VARCHAR(25)  NOT NULL,
  telephone     VARCHAR(15)  NOT NULL,
  email  	      VARCHAR(25)  NOT NULL,
  role     ENUM ('PERFORMER','CREW', 'VENDOR') NOT NULL,

  PRIMARY KEY (person_id)
  UNIQUE KEY `email` (`email`),

) ENGINE=InnoDB;

INSERT INTO `person`(`person_id`, `name`, `telephone`, `email`, `role`) VALUES (1,'Olivia Rodrigo','079635 17383','olivia.rogrigo@gmail.com', 'PERFORMER');
INSERT INTO `person`(`person_id`, `name`, `telephone`, `email`, `role`) VALUES (2,'Kneecap','	00035341 19345', 'kneecap333@outlook.com', 'PERFORMER');


INSERT INTO `person` (`person_id`, `name`, `telephone`, `email`, `role`) 
  VALUES (3, 'Daniel', '079635 17383', 'daniel.kepton@gmail.com', 'CREW');
INSERT INTO `preson` (`person_id`, `name`, `telephone`, `email`, `role`) 
  VALUES (4, 'Adam', '079745 22385', 'adam.johnson@outlook.con', 'CREW');

INSERT INTO `person`(`person_id`, `name`, `telephone`, `email`, `role`) VALUES (5,'Annie','079837 28776','annie.burgersandfries@gmail.com', 'VENDOR');
INSERT INTO `person`(`person_id`, `name`, `telephone`, `email`, `role`) VALUES (6,'Jack','079837 28776','jack.attire@gmail.com', 'VENDOR');
INSERT INTO `person`(`person_id`, `name`, `telephone`, `email`, `role`) VALUES (7,'Luna','079837 28776','luna.fashion@gmail.com', 'VENDOR');

CREATE TABLE performer (
  person_id     INT(10)  NOT NULL AUTO_INCREMENT,
  fee 			INT		 	 NOT NULL,

  	PRIMARY KEY (person_id),

    FOREIGN KEY (person_id) REFERENCES person(person_id)
    ON DELETE CASCADE
    ON UPDATE CASCADE

  CONSTRAINT check_fee CHECK (fee > 0)
) ENGINE=InnoDB;

INSERT INTO `performer`(`person_id`,`fee`) VALUES (1,'5000')
INSERT INTO `performer`(`person_id`,`fee`) VALUES (2,'2000')

CREATE TABLE genre (
  	genre_name    VARCHAR(15)  NOT NULL,

  	PRIMARY KEY (genre_name)

) ENGINE=InnoDB;

INSERT INTO `genre`(`genre_name`) VALUES ('country')
INSERT INTO `genre`(`genre_name`) VALUES ('rock')
INSERT INTO `genre`(`genre_name`) VALUES ('pop')
INSERT INTO `genre`(`genre_name`) VALUES ('hip-hop')

CREATE TABLE genre_performer (
    performer     INT(10)  NOT NULL AUTO_INCREMENT,
    genre_name    VARCHAR(15)  NOT NULL,

  	PRIMARY KEY (performer, genre_name),
    
    FOREIGN KEY (performer) REFERENCES performer(person_id)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
    
    FOREIGN KEY (genre_name) REFERENCES genre(genre_name)
    ON DELETE CASCADE
    ON UPDATE CASCADE

) ENGINE=InnoDB;

INSERT INTO `genre_performer` (`performer`, `genre_name`) VALUES ('1', 'country'), ('2', 'hip-hop')
INSERT INTO `genre_performer` (`performer`, `genre_name`) VALUES ('2', 'rock'), ('2', 'pop')

CREATE TABLE `crew` (
  `person_id` int(10) NOT NULL AUTO_INCREMENT,
  `hourly_rate` int(11) NOT NULL,
  `employment` enum('FULL_TIME','PART_TIME') NOT NULL,
  `weekly_hours` int(11) NOT NULL,

  PRIMARY KEY (`person_id`),

  FOREIGN KEY (person_id) REFERENCES person(person_id)
  ON DELETE CASCADE
  ON UPDATE CASCADE

  CONSTRAINT `check_fee` CHECK (`hourly_rate` > 0),
  CONSTRAINT `check_hours` CHECK 
    (`employment` = 'FULL_TIME' and `weekly_hours` between 25 and 40 or `employment` = 'PART_TIME' and `weekly_hours` between 1 and 24)
) ENGINE=InnoDB 

INSERT INTO `crew` (`person_id`,`hourly_rate`, `employment`, `weekly_hours`) 
  VALUES (3, '21', 'FULL_TIME', '35')
INSERT INTO `crew` (`person_id`, `hourly_rate`, `employment`, `weekly_hours`) 
  VALUES (4, '16', 'PART_TIME', '20')

CREATE TABLE vendor (
  person_id     INT(10)  NOT NULL AUTO_INCREMENT,
  name          VARCHAR(25)  NOT NULL,
  telephone     VARCHAR(15) 	     NOT NULL,
  email  	    VARCHAR(25)  NOT NULL,

  PRIMARY KEY (person_id)

  FOREIGN KEY (person_id) REFERENCES person(person_id)
  ON DELETE CASCADE
  ON UPDATE CASCADE

) ENGINE=InnoDB;

INSERT INTO `vendor`(`person_id`) VALUES (6);
INSERT INTO `vendor`(`person_id`) VALUES (7);
INSERT INTO `vendor`(`person_id`) VALUES (8);

CREATE TABLE product_categories (
  	category    VARCHAR(25)  NOT NULL,

  	PRIMARY KEY (category)

) ENGINE=InnoDB;

INSERT INTO `product_categories`(`product_category`) VALUES ('bandanas')
INSERT INTO `product_categories`(`product_category`) VALUES ('boa buns')
INSERT INTO `product_categories`(`product_category`) VALUES ('burgers')
INSERT INTO `product_categories`(`product_category`) VALUES ('tie die shirts')
INSERT INTO `product_categories`(`product_category`) VALUES ('sunglasses')
INSERT INTO `product_categories`(`product_category`) VALUES ('water bottles')

CREATE TABLE vendor_category (
  	vendor    INT(10) NOT NULL AUTO_INCREMENT,
    category 	  VARCHAR(25)  NOT NULL,

  	PRIMARY KEY (vendor, category),
    
    FOREIGN KEY (vendor) REFERENCES vendor(person_id)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
    
    FOREIGN KEY (category) REFERENCES product_categories(product_category)
    ON DELETE CASCADE
    ON UPDATE CASCADE

) ENGINE=InnoDB;

INSERT INTO `vendor_category` (`vendor`, `category`) VALUES ('1', 'burgers');
INSERT INTO `vendor_category` (`vendor`, `category`) VALUES ('2', 'bandanas'), ('2', 'jumpers'), ('2', 'sunglasses');
INSERT INTO `vendor_category` (`vendor`, `category`) VALUES ('3', 'bandanas'), ('3', 'tie dye shirts'), ('3', 'water bottles');