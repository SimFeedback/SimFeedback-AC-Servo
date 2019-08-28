/*
 rFactorSharedMemoryMap.cpp
 by Dan Allongo (daniel.s.allongo@gmail.com)

 Based on the ISI sample code found at http://rfactor.net/web/rf1/devcorner/
 Provides a basic memory map export of the telemetry and scoring data
 The export is nearly 1:1 with redundant/gratuitous data removed from the 
 vehicle info array and the addition of vehicle speed being pre-calculated
 since everyone needs that anyway. Position, rotation, and orientation are
 interpolated in between scoring updates (every 0.5 seconds). True raw values 
 are given when deltaTime == 0.
*/

#include "RF1SimFeedbackPlugin.hpp"
#include <math.h>
#include <stdio.h>


// plugin information
unsigned g_uPluginID          = 0;
char     g_szPluginName[]     = PLUGIN_NAME;
unsigned g_uPluginVersion     = 002;
unsigned g_uPluginObjectCount = 1;
InternalsPluginInfo g_PluginInfo;

// interface to plugin information
extern "C" __declspec(dllexport)
const char* __cdecl GetPluginName() { return g_szPluginName; }

extern "C" __declspec(dllexport)
unsigned __cdecl GetPluginVersion() { return g_uPluginVersion; }

extern "C" __declspec(dllexport)
unsigned __cdecl GetPluginObjectCount() { return g_uPluginObjectCount; }

// get the plugin-info object used to create the plugin.
extern "C" __declspec(dllexport)
PluginObjectInfo* __cdecl GetPluginObjectInfo( const unsigned uIndex ) {
  switch(uIndex) {
    case 0:
      return  &g_PluginInfo;
    default:
      return 0;
  }
}


// InternalsPluginInfo class

InternalsPluginInfo::InternalsPluginInfo() {
  // put together a name for this plugin
  sprintf_s( m_szFullName, "%s - %s", g_szPluginName, InternalsPluginInfo::GetName() );
}

const char*    InternalsPluginInfo::GetName()     const { return SharedMemoryMapPlugin::GetName(); }
const char*    InternalsPluginInfo::GetFullName() const { return m_szFullName; }
const char*    InternalsPluginInfo::GetDesc()     const { return g_szPluginName; }
const unsigned InternalsPluginInfo::GetType()     const { return SharedMemoryMapPlugin::GetType(); }
const char*    InternalsPluginInfo::GetSubType()  const { return SharedMemoryMapPlugin::GetSubType(); }
const unsigned InternalsPluginInfo::GetVersion()  const { return SharedMemoryMapPlugin::GetVersion(); }
void*          InternalsPluginInfo::Create()      const { return new SharedMemoryMapPlugin(); }


// InternalsPlugin class

const char SharedMemoryMapPlugin::m_szName[] = PLUGIN_NAME;
const char SharedMemoryMapPlugin::m_szSubType[] = "Internals";
const unsigned SharedMemoryMapPlugin::m_uID = 1;
const unsigned SharedMemoryMapPlugin::m_uVersion = 3;  // set to 3 for InternalsPluginV3 functionality and added graphical and vehicle info

PluginObjectInfo *SharedMemoryMapPlugin::GetInfo() {
  return &g_PluginInfo;
}

void SharedMemoryMapPlugin::Startup() {
	// init handle and try to create, read if existing
	hMap = INVALID_HANDLE_VALUE;
	hMap = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, sizeof(RF1Data), TEXT(SHARED_MEMORY));
	if (hMap == NULL) {
		if (GetLastError() == (DWORD)183) {
			hMap = OpenFileMapping(FILE_MAP_ALL_ACCESS, FALSE, TEXT(SHARED_MEMORY));
			if (hMap == NULL) {
				// unable to create or read existing
				mapped = FALSE;
				return;
			}
		}
	}
	data = (RF1Data*)MapViewOfFile(hMap, FILE_MAP_ALL_ACCESS, 0, 0, sizeof(RF1Data));
	if (data == NULL) {
		// failed to map memory buffer
		CloseHandle(hMap);
		mapped = FALSE;
		return;
	}
	mapped = TRUE;
	if (mapped) {
		memset(data, 0, sizeof(RF1Data));
	}
	return;
}

void SharedMemoryMapPlugin::Shutdown() {
	// release buffer and close handle
	if (mapped) {
		memset(data, 0, sizeof(RF1Data));
	}
	if (data) {
		UnmapViewOfFile(data);
	}
	if (hMap) {
		CloseHandle(hMap);
	}
	mapped = FALSE;
}

void SharedMemoryMapPlugin::StartSession() {
	// zero-out buffer at start of session
	if (mapped) {
		memset(data, 0, sizeof(RF1Data));
	}
	cLastTelemUpdate = 0;
	cLastScoringUpdate = 0;
	cDelta = 0;
	inSession = TRUE;
}

void SharedMemoryMapPlugin::EndSession() {
	// zero-out buffer at end of session
	StartSession();
	inSession = FALSE;
}

void SharedMemoryMapPlugin::EnterRealtime() {
	inRealtime = TRUE;
}

void SharedMemoryMapPlugin::ExitRealtime() {
	inRealtime = FALSE;
}

void SharedMemoryMapPlugin::UpdateTelemetry( const TelemInfoV2 &info ) {
	if (mapped) {

		data->time = (float)clock();

		// Compute some auxiliary info based on the above
		TelemVect3 forwardVector = { -info.mOriX.z, -info.mOriY.z, -info.mOriZ.z };
		TelemVect3    leftVector = { info.mOriX.x,  info.mOriY.x,  info.mOriZ.x };

		// These are normalized vectors, and remember that our world Y coordinate is up.  So you can
		// determine the current pitch and roll (w.r.t. the world x-z plane) as follows:
		const double pitch = atan2(forwardVector.y, sqrt((forwardVector.x * forwardVector.x) + (forwardVector.z * forwardVector.z)));
		const double  roll = atan2(leftVector.y, sqrt((leftVector.x * leftVector.x) + (leftVector.z * leftVector.z)));
		const double radsToDeg = 57.296;
		data->pitch = pitch;
		data->roll = roll;
		//fprintf(logFile, "Pitch = %.1f deg, Roll = %.1f deg\n", pitch * radsToDeg, roll * radsToDeg);

		data->accelX = info.mLocalAccel.x;
		data->accelY = info.mLocalAccel.y;
		data->accelZ = info.mLocalAccel.z;
		//fprintf(logFile, "AccelX = %.2f m/s^2, AccelY = %.2f m/s^2, AccelZ = %.2f m/s^2", data->accelX, data->accelY, data->accelZ);

		data->rotAccelX = info.mLocalRotAccel.x;
		data->rotAccelX = info.mLocalRotAccel.y;
		data->rotAccelX = info.mLocalRotAccel.z;
		//fprintf(logFile, "RotAccelX = %.2f rad/s^2, RotAccelY = %.2f rad/s^2, RotAccelZ = %.2f rad/s^2", data->rotAccelX, data->rotAccelY, data->rotAccelZ);

		data->velX = info.mLocalVel.x;
		data->velY = info.mLocalVel.y;
		data->velZ = info.mLocalVel.z;
		//fprintf(logFile, "VelX = %.2f m/s, VelY = %.2f m/s, VelZ = %.2f m/s", data->velX, data->velY, data->velZ);

		data->rotVelX = info.mLocalRot.x;
		data->rotVelY = info.mLocalRot.y;
		data->rotVelZ = info.mLocalRot.z;
		//fprintf(logFile, "RotVelX = %.2f rad/s, RotVelY = %.2f rad/s, RotVelZ = %.2f rad/s", data->rotVelX, data->rotVelY, data->rotVelZ);

		const double metersPerSec = sqrt((info.mLocalVel.x * info.mLocalVel.x) +
			(info.mLocalVel.y * info.mLocalVel.y) +
			(info.mLocalVel.z * info.mLocalVel.z));
		//fprintf(logFile, "Speed = %.1f KPH, %.1f MPH\n\n", metersPerSec * 3.6, metersPerSec * 2.237);

		data->speed = metersPerSec;

		data->rpm = info.mEngineRPM;
	}
}

void SharedMemoryMapPlugin::UpdateScoring( const ScoringInfoV2 &info ) {
}
