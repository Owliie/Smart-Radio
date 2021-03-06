#ifndef __DEFINITIONS_H__
#define __DEFINITIONS_H__

#define URL_SELECTION 0

#if URL_SELECTION == 0
#define STREAM_URL "http://46.10.150.243/z-rock.mp3"
#elif URL_SELECTION == 1
#define STREAM_URL "http://78.31.65.20:8080/dance.mp3"
#elif URL_SELECTION == 2
#define STREAM_URL "http://play.global.audio:80/nrj128"
#elif URL_SELECTION == 3
#define STREAM_URL "http://193.108.24.21:8000/fresh"
#elif URL_SELECTION == 4
#define STREAM_URL "http://46.10.150.243:80/njoy.mp3"
#endif

#define MUSIC_RECOGNITION_BASE_URL "https://192.168.0.101:5000"
#define MUSIC_RECOGNITION_BASE_URL_NO_PORT "192.168.0.101"

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

#define PIN_PROMPT_RECORD_BUTTON GPIO_NUM_35
#define PIN_UPDATE_VALUE_BUTTON GPIO_NUM_14
#define PIN_CONFIRM_VALUE_BUTTON GPIO_NUM_34
#define PIN_PIEZO GPIO_NUM_4

#define SNIPPET_SIZE 250000

#define SOUND_PWM_CHANNEL 0
#define SOUND_RESOLUTION 8
#define SOUND_ON (1 << (SOUND_RESOLUTION - 1))
#define SOUND_OFF 0

#define OLED_ICON_WIDTH 10
#define OLED_ICON_HEIGHT 10

#endif