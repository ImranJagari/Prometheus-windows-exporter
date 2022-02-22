# Prometheus windows exporter

this exporter is an executable that launch a webserver to deserve data from windows hosts for prometheus processing.

## Grafana Dashboard
I've made a grafana dashboard with this exporter, feel free to download it, rate it and let a comment to improve it 
https://grafana.com/grafana/dashboards/15652

## Settings

Every data is configurable with appsettings.json file
Example : 
```json
{
  "IISLogTask": {
    "Enabled": false,
    "PrefixKeyName": "windows_iis_logs",
    "PathToLogs": "C:\\Logs"
  },
  "PerformanceTask": {
    "Enabled": false,
    "Categories": {
      "Processor": {
        "Enabled": true
      },
      "Processor Information": {
        "Enabled": true
      },
      "Memory": {
        "Enabled": true
      },
      "NUMA Node Memory": {
        "Enabled": true
      },
      "Web Service": {
        "Enabled": true
      },
      "LogicalDisk": {
        "Enabled": true
      }
    }
  },
  "WMITask": {
    "Enabled": false,
    "PrefixKeyName": "windows_management",
    "Searchers": {
      "Win32_LogicalDisk": {
        "Enabled": true
      }
    }
  },
  "ComputerSystemTask": {
    "Enabled": false,
    "PrefixKeyName": "windows_cs"
  },
  "IISTask": {
    "Enabled": false,
    "PrefixKeyName": "windows_iis_servers"
  },
  "ServicesWatcherTask": {
    "Enabled": false,
    "PrefixKeyName": "windows_system_services_status",
    "Services": [
      {
        "ServiceName": "TestServiceName",
        "CanRestart": false
      }
    ]
  }
}
```


## IIS Log Task

This task retrieve all events from the folder where IIS log everything about your server and your sites.
You must configure your IIS logging to W3C format

Example :
```json
"IISLogTask": {
  "Enabled": false,
  "PrefixKeyName": "windows_iis_logs",
  "PathToLogs": "C:\\Logs"
}
```

- ``"Enabled"`` is for enabling the task
- ``"PrefixKeyName"`` is the perfix used on exported data for prometheus
- ``"PathToLogs"`` is the path to iis logs root folder, by default it's on this path ``C:\inetpub\logs\LogFiles``


## Performance Task

This task query the performance monitor to get data from Operating system as CPU, Memory, Disk,...
Check the performance monitor to get all categories and counters names from your system, no need to translate the names to english if you system is in another language, you can put in the configuration like it's presented.

Example:
```json
"PerformanceTask": {
  "Enabled": false,
  "Categories": {
    "Processor": {
      "Enabled": true,
      "MetricType": "counter"
    },
    "Processor Information": {
      "Enabled": true,
      "MetricType": "gauge"
    },
  }
}
```

- ``"Enabled"`` is for enabling the task
- ``"Categories"`` is a dictionary taking performance counter name as key and object with 2 properties ``"Enabled"`` which is for enabling the watcher on the counter and ``"MetricType"`` which you can configure the data type for prometheus, it takes 4 values : ``counter``, ``gauge``, ``histogram``, ``summary``

more informations about performance counters : https://kb.paessler.com/en/topic/50673-how-can-i-find-out-the-names-of-available-performance-counters


## WMI Task

This task query the Windows Management system to gather all hardware and operating system values.

Example:
```json
"WMITask": {
  "Enabled": false,
  "PrefixKeyName": "windows_management",
  "Searchers": {
    "Win32_LogicalDisk": {
      "Enabled": true
    }
  }
}
```

- ``"Enabled"`` is for enabling the task
- ``"PrefixKeyName"`` is the perfix used on exported data for prometheus
- ``"Searchers"`` is a dictionary taking windows management object name as key and object with 2 properties ``"Enabled"`` which is for enabling the watcher on the wm object and ``"MetricType"`` which you can configure the data type for prometheus, it takes 4 values : ``counter``, ``gauge``, ``histogram``, ``summary``

You can check this thread on SO (https://stackoverflow.com/questions/27227125/where-can-i-find-all-tables-used-in-managementobjectsearcher-in-win32-api) 
or in MSDN on the Win32 section (https://docs.microsoft.com/fr-fr/windows/win32/cimwin32prov/computer-system-hardware-classes)


## ComputerSystemTask

This task get informations from system as hostname and domain, it can be improved by adding more datas

Example:
```json
"ComputerSystemTask": {
  "Enabled": false,
  "PrefixKeyName": "windows_cs"
}
```

- ``"Enabled"`` is for enabling the task
- ``"PrefixKeyName"`` is the perfix used on exported data for prometheus


## IISTask

This task get all data from the IIS servers configuration with the package Microsoft.Web.Administration, it retrieve iis applications and pool datas

Example:
```json
"IISTask": {
  "Enabled": false,
  "PrefixKeyName": "windows_iis_servers"
}
```

- ``"Enabled"`` is for enabling the task
- ``"PrefixKeyName"`` is the perfix used on exported data for prometheus


## ServicesWatcherTask

This task is for windows services, it search by the service name to retrieve its status and you can configure it, to restart it if you want.

Example:
```json
"ServicesWatcherTask": {
  "Enabled": false,
  "PrefixKeyName": "windows_system_services_status",
  "Services": [
    {
      "ServiceName": "TestServiceName",
      "CanRestart": false
    }
  ]
}
```

- ``"Enabled"`` is for enabling the task
- ``"PrefixKeyName"`` is the perfix used on exported data for prometheus
- ``"Services"`` is a list of service composed by ``"ServiceName"`` which is the service real name and ``"CanRestart"`` which indicate if the service can be restarted by this program


# Credits

Thanks to 
  - Kabindas for his iis log parser (https://github.com/Kabindas/IISLogParser)
