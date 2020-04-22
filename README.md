### Addon

Addon ZoomStatus for IOT Link allow to monitor Zoom for active meeting of current user. It was written to track my kids are they are in Zoom so they will not miss classes. Full source is here.

<img src="status.png" width="350">

### Installation

- Create folder ZoomStatus under %ProgramData%\\IOTLink\\Addons
- Copy ZoomStatus.dll from Addons\\ZoomStatus\\bin\\Release to created folder ZoomStatus
- Copy addon.yaml from Addons\\ZoomStatus to created folder ZoomStatus
- Restart IOTLink service

Place next configuration to Home Assistant into "Sensors" section, replace "workgroup/my-computer" with your one:
```yaml
- platform: mqtt
  name: zoom_meeting_name
  unit_of_measurement: ''
  state_topic: "iotlink/workgroup/my-computer/zoom-status/zoom-status"
  value_template: "{{ value }}"
  availability_topic: "iotlink/workgroup/my-computer/lwt"
  payload_available: "ON"
  payload_not_available: "OFF"
  qos: 1

- platform: template
  sensors:
    zoom_meeting:
      friendly_name: "Zoom Meeting"
      unit_of_measurement: ''
      icon_template: >-
        {% if is_state('sensor.zoom_meeting_name', 'no') %}
          mdi:account-tie-voice-off
        {% else %}
          mdi:account-tie-voice
        {% endif %}
      value_template: '{{ states("sensor.zoom_meeting_name") }}'
```

![](https://gitlab.com/iotlink/iotlink/raw/develop/Assets/images/logos/logo_full.png)

IOT Link is a full featured service for connecting devices with IOT enabled services using MQTT.

### Features

- Open source software.
- Easy Installer provided.
- Minimal configuration setup.
- Changes to engine configuration will be reloaded **automatically**.
- Widely customizable using [Addons](https://gitlab.com/iotlink/iotlink/wikis/Addons/Home).
- Fully working as a **Windows Service** to provide reliable data as soon as the windows boot up, without having to wait for a windows user logon.

### Requirements

- Windows 10 with **administrator** rights.
- [.NET Framework 4.7.2](https://dotnet.microsoft.com/download/dotnet-framework/net472)
- Minimal understanding of YAML files.
- **Observations:**
  - It might work with other Windows version, however it has been developed and tested only on Windows 10 (Update 1803+).
  - Please if you have it fully working on other Windows versions, inform it commenting on the following Issue: [Working Windows Versions](#2).

### More Information.

- More information available at our [Wiki Pages](https://gitlab.com/iotlink/iotlink/wikis/).
