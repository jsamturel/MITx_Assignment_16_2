
#  .--..--..--..--..--..--..--..--..--..--. 
# / .. \.. \.. \.. \.. \.. \.. \.. \.. \.. \
# \ \/\ `'\ `'\ `'\ `'\ `'\ `'\ `'\ `'\ \/ /
#  \/ /`--'`--'`--'`--'`--'`--'`--'`--'\/ / 
#  / /\                                / /\ 
# / /\ \      _____           _       / /\ \
# \ \/ /   __|_   _|__   ___ | |___   \ \/ /
#  \/ /   / __|| |/ _ \ / _ \| / __|   \/ / 
#  / /\  | (__ | | (_) | (_) | \__ \   / /\ 
# / /\ \  \___||_|\___/ \___/|_|___/  / /\ \
# \ \/ /                              \ \/ /
#  \/ /                                \/ / 
#  / /\.--..--..--..--..--..--..--..--./ /\ 
# / /\ \.. \.. \.. \.. \.. \.. \.. \.. \/\ \
# \ `'\ `'\ `'\ `'\ `'\ `'\ `'\ `'\ `'\ `' /
#  `--'`--'`--'`--'`--'`--'`--'`--'`--'`--' 

Thank you for downloading CTimer package.

--- HOW TO USE ---

-> You can create TimerManager object to scene from Tools/Create Time Manager from the menu bar.
Please don't forget to create the TimerManager object on the game scene. You can also manually add the 
TimerManager script to any gameObject you want on the game scene.

-> Use TimerManager.Timer(/*duration of the timer*/) to create new timer. 

-> You can subscribe the onComplete and onStart events from Timer.OnComplete() & Timer.OnStart methods.



----- v1.0.1 // World Time Update \\ -----

	Added new WorldTime and UserTimeData classes.

	WorldTime sends web requests to get utc time data. You can get utc time data from
TimerManager.WorldTime.OnTimeFetchComplete event fired. 

	You can also get current world time after it fetched successfully from the property:
 => TimerManager.WorldTime.UTCTime.Value

	You can get first install time of the application on the user end with:
=> TimerManager.UserTimeData.FirstInstallTime.Value

	You can get TimeSpan from first install of the application on the user end with: 
=> TimerManager.UserTimeData.GetPassedTimeSinceFirstInstall().Value;

	!!! Checking the null value for all the values is recommended since the web request may not be
completed. It may be because of the connection on the user's end or the api address which I use
to get the UTC world time.

	Example: if(TimerManager.WorldTime.UTCTime.HasValue == false) => data have not been fetched yet
or user has no internet connection.

-------------------



Please ask me anything about the package from : 

ozkntn@gmail.com
https://www.linkedin.com/in/ozkantan1/
https://assetstore.unity.com/publishers/80556