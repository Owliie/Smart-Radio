#ifndef __DEFINITIONS_H__
#define __DEFINITIONS_H__

#define URL_SELECTION 2

#if URL_SELECTION == 0
#define STREAM_URL "http://46.10.150.243/z-rock.mp3"
#elif URL_SELECTION == 1
#define STREAM_URL "http://78.31.65.20:8080/dance.mp3"
#elif URL_SELECTION == 2
#define STREAM_URL "http://play.global.audio:80/nrj128"
#elif URL_SELECTION == 3
#define STREAM_URL "http://play.global.audio/veronika128"
#endif

#define WIFI_SSID "Gerginov"
#define WIFI_PASSWORD "59199878"

#define MAX_STACK_SIZE 10000

#define MOUNT_POINT_FAT "/fatflash"

#define PIN_SPI_MOSI GPIO_NUM_23 // PIN 5 - IO0 - DI
#define PIN_SPI_MISO GPIO_NUM_19 // PIN 2 - IO1 - DO
#define PIN_SPI_WP GPIO_NUM_22   // PIN 3 - IO2 - /WP
#define PIN_SPI_HD GPIO_NUM_21   // PIN 7 - IO3 - /HOLD - /RESET
#define PIN_SPI_SCK GPIO_NUM_18  // PIN 6 - CLK - CLK
#define PIN_SPI_SS GPIO_NUM_5    // PIN 1 - /CS - /CS

#endif