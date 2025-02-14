let selectedTeam = 0;

// Called when a new room is selected on the sidebar
function roomSelect(inputVal){
    var elem = document.getElementsByClassName("roomListItemBody");

    for ( var i = 0; i < elem.length; i++) 
    {
        elem[i].style.borderStyle = 'none';
    }
        
    roomBorder(inputVal);

    // minus 1 handles the zero indexing of the team number folders
    swapTeams(inputVal);
}

// Puts border around Room name and number when selected
function roomBorder(num) {

    var roomId = "room" + (num);
    document.getElementById(roomId).style.borderColor = 'rgba(255, 255, 255, 1)';
    document.getElementById(roomId).style.borderWidth = '.05rem';
    document.getElementById(roomId).style.borderStyle = 'solid';
}

// Sets the cookie to remember the last team selected
function setCookie(cname, cvalue, exdays) {

    const d = new Date();
    d.setTime(d.getTime() + (exdays*24*60*60*1000));
    let expires = "expires=" + d.toUTCString();

    // Sets cookie value for the room num and gives it an expiration date
    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
}

// Updates team specific data when another team is selected
function swapTeams(newTeam){

    // Update global team
    selectedTeam = newTeam - 1;

    // Reload the team specific Json files
    loadPR_Telemetry(selectedTeam);

    // Assign page title based on room selected
    var room = "room" + (selectedTeam + 1) + "Name";
    document.getElementById('roomDataTitle').textContent = document.getElementById(room).innerText + " - Room " + (selectedTeam + 1);

    // Sets the cookie anytime a new team is selected
    setCookie("roomNum", selectedTeam, 1);
}

// Load telemetry of Pressurized Rover
function loadPR_Telemetry(team){
    $.getJSON("json_data/teams/" + team + "/ROVER_TELEMETRY.json", function ( data ){
        if(data.pr_telemetry.ac_heating == true){
            document.getElementById("acHeatingSensor").style.backgroundColor = 'rgba(0, 240, 10, 1)';
            document.getElementById("acHeatingSwitch").checked = true;
        }
        else{
            document.getElementById("acHeatingSensor").style.backgroundColor = 'rgba(100, 100, 100, 1)';
            document.getElementById("acHeatingSwitch").checked = false;
        } 

        if(data.pr_telemetry.ac_cooling == true){
            document.getElementById("acCoolingSensor").style.backgroundColor = 'rgba(0, 240, 10, 1)';
            document.getElementById("acCoolingSwitch").checked = true;

        }
        else{
            document.getElementById("acCoolingSensor").style.backgroundColor = 'rgba(100, 100, 100, 1)';
            document.getElementById("acCoolingSwitch").checked = false;
        }
        
        if(data.pr_telemetry.lights_on == true){
            document.getElementById("lightsOnSensor").style.backgroundColor = 'rgba(0, 240, 10, 1)';
            document.getElementById("lightsOnSwitch").checked = true;
        }
        else{
            document.getElementById("lightsOnSensor").style.backgroundColor = 'rgba(100, 100, 100, 1)';
            document.getElementById("lightsOnSwitch").checked = false;
        } 

        if(data.pr_telemetry.internal_lights_on == true){
            document.getElementById("internalLightsSensor").style.backgroundColor = 'rgba(0, 240, 10, 1)';
            document.getElementById("internalLightsSwitch").checked = true;
        }
        else{
            document.getElementById("internalLightsSensor").style.backgroundColor = 'rgba(100, 100, 100, 1)';
            document.getElementById("internalLightsSwitch").checked = false;
        } 

        if(data.pr_telemetry.brakes == true){
            document.getElementById("brakesSensor").style.backgroundColor = 'rgba(0, 240, 10, 1)';
            document.getElementById("brakesSwitch").checked = true;
        }
        else{
            document.getElementById("brakesSensor").style.backgroundColor = 'rgba(100, 100, 100, 1)';
            document.getElementById("brakesSwitch").checked = false;
        } 

        if(data.pr_telemetry.in_sunlight == true){
            document.getElementById("inSunlightSensor").style.backgroundColor = 'rgba(0, 240, 10, 1)';
            document.getElementById("inSunlightSwitch").checked = true;
        }
        else{
            document.getElementById("inSunlightSensor").style.backgroundColor = 'rgba(100, 100, 100, 1)';
            document.getElementById("inSunlightSwitch").checked = false;
        }

        if(data.pr_telemetry.co2_scrubber == true){
            document.getElementById("co2ScrubberSensor").style.backgroundColor = 'rgba(0, 240, 10, 1)';
            document.getElementById("co2ScrubberSwitch").checked = true;
        }
        else{
            document.getElementById("co2ScrubberSensor").style.backgroundColor = 'rgba(100, 100, 100, 1)';
            document.getElementById("co2ScrubberSwitch").checked = false;
        }

        if(data.pr_telemetry.dust_wiper == true){
            document.getElementById("dustWiperSensor").style.backgroundColor = 'rgba(0, 240, 10, 1)';
            document.getElementById("dustWiperSwitch").checked = true;
        }
        else{
            document.getElementById("dustWiperSensor").style.backgroundColor = 'rgba(100, 100, 100, 1)';
            document.getElementById("dustWiperSwitch").checked = false;
        }

        if(data.pr_telemetry.fan_pri == true){
            document.getElementById("fanPriSensor").style.backgroundColor = 'rgba(0, 240, 10, 1)';
            document.getElementById("fanPriSwitch").checked = true;
        }
        else{
            document.getElementById("fanPriSensor").style.backgroundColor = 'rgba(100, 100, 100, 1)';
            document.getElementById("fanPriSwitch").checked = false;
        }

        if(data.pr_telemetry.switch_dest == true){
            document.getElementById("switchDestSensor").style.backgroundColor = 'rgba(0, 240, 10, 1)';
            document.getElementById("switchDestSwitch").checked = true;
        }
        else{
            document.getElementById("switchDestSensor").style.backgroundColor = 'rgba(100, 100, 100, 1)';
            document.getElementById("switchDestSwitch").checked = false;
        }

        let throttle = data.pr_telemetry.throttle;
        let steering = data.pr_telemetry.steering;
        let current_pos_x = data.pr_telemetry.current_pos_x;
        let current_pos_y = data.pr_telemetry.current_pos_y;
        let current_pos_alt = data.pr_telemetry.current_pos_alt;
        let heading = data.pr_telemetry.heading;
        let roll = data.pr_telemetry.roll;
        let pitch = data.pr_telemetry.pitch;
        let distance_traveled = data.pr_telemetry.distance_traveled;
        let speed = data.pr_telemetry.speed;
        let surface_incline = data.pr_telemetry.surface_incline;

        let battery_level = data.pr_telemetry.battery_level;
        let solar_panel_dust_accum = data.pr_telemetry.solar_panel_dust_accum;
        let solar_panel_efficiency = data.pr_telemetry.solar_panel_efficiency;

        let oxygen_levels = data.pr_telemetry.oxygen_levels;
        let oxygen_tank = data.pr_telemetry.oxygen_tank;
        let oxygen_pressure = data.pr_telemetry.oxygen_pressure;
        let ac_fan_pri = data.pr_telemetry.ac_fan_pri;
        let ac_fan_sec = data.pr_telemetry.ac_fan_sec;

        let cabin_pressure = data.pr_telemetry.cabin_pressure;
        let cabin_temperature = data.pr_telemetry.cabin_temperature;

        // Convert the porcentage consumption rate to watts
        // Total_battery_capacity is assuming same capacity as an electric car in Joules
        let total_battery_capacity = 4320000;

        // Power_consumption_rate is now in kiloWatt/hour
        let power_consumption_rate = ((total_battery_capacity * data.pr_telemetry.power_consumption_rate / 100) / 1000) * 3600;

        let motor_power_consumption = ((total_battery_capacity * data.pr_telemetry.motor_power_consumption / 100) / 1000) * 3600;

        let external_temp = data.pr_telemetry.external_temp;
        let coolant_level = data.pr_telemetry.pr_coolant_level;
        let coolant_pressure = data.pr_telemetry.pr_coolant_pressure;
        let coolant_tank = data.pr_telemetry.pr_coolant_tank;
        let terrain_condition = data.pr_telemetry.terrain_condition;
        let mission_elapsed_time = data.pr_telemetry.mission_elapsed_time;
        let mission_planned_time = data.pr_telemetry.mission_planned_time;
        let point_of_no_return = data.pr_telemetry.point_of_no_return;
        let distance_from_base = data.pr_telemetry.distance_from_base;

        //Destination
        let switch_dest = data.pr_telemetry.switch_dest;
        let dest_x = data.pr_telemetry.dest_x;
        let dest_y = data.pr_telemetry.dest_y;
        let dest_z = data.pr_telemetry.dest_z;


        // PR Positioning
        document.getElementById("throttle").innerText = throttle.toFixed(2) + " %";
        document.getElementById("steering").innerText = steering.toFixed(2);
        document.getElementById("current_pos_x").innerText = current_pos_x.toFixed(2);
        document.getElementById("current_pos_y").innerText = current_pos_y.toFixed(2);
        document.getElementById("current_pos_alt").innerText = current_pos_alt.toFixed(2);
        document.getElementById("heading").innerText = heading.toFixed(2) + " rads";
        document.getElementById("roll").innerText = roll.toFixed(2) + " rads";
        document.getElementById("pitch").innerText = pitch.toFixed(2) + " rads";
        document.getElementById("distance_traveled").innerText = distance_traveled.toFixed(2) + " m";
        document.getElementById("speed").innerText = speed.toFixed(2) + " m/s";
        document.getElementById("surface_incline").innerText = surface_incline.toFixed(2) + " rads";
        document.getElementById("oxygen_pressure").innerText = oxygen_pressure.toFixed(2) + " psi";
        document.getElementById("oxygen_levels").innerText = oxygen_levels.toFixed(2) + " %";
        document.getElementById("oxygen_tank").innerText = oxygen_tank.toFixed(2) + " %";
        document.getElementById("solar_panel_dust_accum").innerText = solar_panel_dust_accum.toFixed(2) + " %";
        document.getElementById("battery_level").innerText = battery_level.toFixed(2) + " %";
        document.getElementById("ac_fan_pri").innerText = ac_fan_pri.toFixed(2) + " rpm";
        document.getElementById("ac_fan_sec").innerText = ac_fan_sec.toFixed(2) + " rpm";
        document.getElementById("cabin_pressure").innerText = cabin_pressure.toFixed(2) + " psi";
        document.getElementById("cabin_temperature").innerText = cabin_temperature.toFixed(2) + " °C";
        document.getElementById("power_consumption_rate").innerText = power_consumption_rate.toFixed(2) + " kWh";
        document.getElementById("solar_panel_efficiency").innerText = solar_panel_efficiency.toFixed(2);
        document.getElementById("external_temp").innerText = external_temp.toFixed(2) + " °C";
        document.getElementById("pr_coolant_level").innerText = coolant_level.toFixed(2) + " L";
        document.getElementById("pr_coolant_pressure").innerText = coolant_pressure.toFixed(2) + " psi";
        document.getElementById("pr_coolant_tank").innerText = coolant_tank.toFixed(2) + " %";
        document.getElementById("motor_power_consumption").innerText = motor_power_consumption.toFixed(2) + " kWh";
        document.getElementById("terrain_condition").innerText = terrain_condition.toFixed(2);
        document.getElementById("mission_elapsed_time").innerText = mission_elapsed_time.toFixed(2);
        document.getElementById("mission_planned_time").innerText = mission_planned_time.toFixed(2);
        document.getElementById("point_of_no_return").innerText = point_of_no_return.toFixed(2);
        document.getElementById("distance_from_base").innerText = distance_from_base.toFixed(2) + " m";

        //Destination
        //document.getElementById("pr_switch_dest").innerText = switch_dest;
        document.getElementById("dest_x").innerText = dest_x.toFixed(2);
        document.getElementById("dest_y").innerText = dest_y.toFixed(2);
        document.getElementById("dest_z").innerText = dest_z.toFixed(2);
    });
}

// Loads team names for Rooms
function loadTeams() {

    $.getJSON("json_data/TEAMS.json", function (data) {
        document.getElementById('room1Name').innerText = data.teams.team_1;
        document.getElementById('room2Name').innerText = data.teams.team_2;
        document.getElementById('room3Name').innerText = data.teams.team_3;
        document.getElementById('room4Name').innerText = data.teams.team_4;
        document.getElementById('room5Name').innerText = data.teams.team_5;
        document.getElementById('room6Name').innerText = data.teams.team_6;
        document.getElementById('room7Name').innerText = data.teams.team_7;
        document.getElementById('room8Name').innerText = data.teams.team_8;
        document.getElementById('room9Name').innerText = data.teams.team_9;
        document.getElementById('room10Name').innerText = data.teams.team_10;
        document.getElementById('room11Name').innerText = data.teams.team_11;

        prRunningIndex = -1;
        prRunningTeam = "";
        for (var i = 0; i < data.teams.team_num; i++) {
            loadLights(i);
        }
        
        if (prRunningIndex >= 0) {
            prRunningTeam =  document.getElementById('room' + (prRunningIndex + 1) + 'Name').innerText
        }
    })
}

// Loads lights for Rooms depending on state
function loadLights(team) {
        $.getJSON("json_data/teams/" + team + "/ROVER_TELEMETRY.json", function( pr_data ){

        // Button UI States Visuals
        var evaStarted  = pr_data.pr_telemetry.sim_running;
        var evaComplete = pr_data.pr_telemetry.completed;

        // Team light state
        var room = "room" + (team + 1) + "Light";
        var roomLight = document.getElementById(room);
        if(evaStarted){
            roomLight.style.backgroundColor = 'rgba(0, 240, 10, 1)';
        } 
        else if(evaComplete){
            roomLight.style.backgroundColor = 'rgba(0, 0, 255, 1)';
        } 
        else {
            roomLight.style.backgroundColor = 'rgba(100, 100, 100, 1)';
        }
    });
}

function onload() {

    // Load immediately
    loadPR_Telemetry(selectedTeam);
    loadTeams();

    // Continously refreshes values
	setInterval(function() {

        loadPR_Telemetry(selectedTeam);

        loadLights(selectedTeam);


    }, 1000);
}