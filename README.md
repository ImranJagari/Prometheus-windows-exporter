# Prometheus windows exporter

this exporter is an executable that launch a webserver to deserve data from windows hosts for prometheus processing.

## Settings

Every data is configurable with appsettings.json file
```json
{
  "IISLogTask": {
    "Enabled": false, // if enabled is setted to false, the task won't run
    "PathToLogs": "Path to your iis log folder"
  },
  "PerformanceTask": {
    "Enabled": false,
    "Categories": {
      "Performance category name example": {
        "Enabled": true,
        "Counters": {
          "Counter name inside the category"
        }
      },
      "Another performance category": {
        "Enabled": true
        //if the counter section is not setted, all counters are retrieved
      }
    }
  },
  "WMITask": {
    "Enabled": true,
    "Searchers": {
      "Windows management searcher name": {
        "Enabled": true,
        "Properties":{
          "Enabled": true,
          //Same logic as PerformanceTask, just the name of base sections are changed
        }
      }
    }
  }
}
```


## IIS Log Task

This task retrieve all events from the folder where IIS log everything about your server and your sites.
You must configure your IIS logging to W3C format


## Performance Task

This task query the performance monitor to get data from Operating system as CPU, Memory, Disk,...
Check the performance monitor to get all categories and counters names from your system, no need to translate the names to english if you system is in another language, you can put in the configuration like it's presented.

## WMI Task

This task query the Windows Management system to gather all hardware and operating system values.
You can check this thread on SO (https://stackoverflow.com/questions/27227125/where-can-i-find-all-tables-used-in-managementobjectsearcher-in-win32-api) 
or in MSDN on the Win32 section (https://docs.microsoft.com/fr-fr/windows/win32/cimwin32prov/computer-system-hardware-classes)

# Credits

Thanks to 
  - Kabindas for his iis log parser (https://github.com/Kabindas/IISLogParser)
