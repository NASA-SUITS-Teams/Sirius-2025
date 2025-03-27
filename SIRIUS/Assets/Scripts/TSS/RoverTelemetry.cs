using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PrTelemetry
{
    public bool ac_heating;
    public bool ac_cooling;
    public bool co2_scrubber;
    public bool lights_on;
    public bool internal_lights_on;
    public bool brakes;
    public bool in_sunlight;
    public int throttle;
    public int steering;
    public int current_pos_x;
    public int current_pos_y;
    public int current_pos_alt;
    public int heading;
    public int pitch;
    public int roll;
    public int distance_traveled;
    public int speed;
    public int surface_incline;
    public float oxygen_tank;
    public float oxygen_pressure;
    public float oxygen_levels;
    public bool fan_pri;
    public int ac_fan_pri;
    public int ac_fan_sec;
    public int cabin_pressure;
    public float cabin_temperature;
    public float battery_level;
    public float power_consumption_rate;
    public int solar_panel_efficiency;
    public int external_temp;
    public float pr_coolant_level;
    public float pr_coolant_pressure;
    public float pr_coolant_tank;
    public int radiator;
    public int motor_power_consumption;
    public int terrain_condition;
    public float solar_panel_dust_accum;
    public int mission_elapsed_time;
    public int mission_planned_time;
    public int point_of_no_return;
    public int distance_from_base;
    public bool switch_dest;
    public int dest_x;
    public int dest_y;
    public int dest_z;
    public bool dust_wiper;
    public bool sim_running;
    public bool sim_paused;
    public bool sim_completed;
    public List<float> lidar;
}

[System.Serializable]
public class RoverTelemetry
{
    public PrTelemetry pr_telemetry;
}
