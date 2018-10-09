//
// Copyright (c) 2018 Rausch IT
//
// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in 
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
// THE SOFTWARE.
//
//
#include "RF2SimFeedbackPlugin.hpp"
#include "RF2Data.hpp"

#include <stdio.h>              // for sample output
#include <string>
#include <time.h>

#pragma warning(disable : 4996) //_CRT_SECURE_NO_WARNINGS

// plugin information


extern "C" __declspec(dllexport)
const char * __cdecl GetPluginName() { return("RF2SimFeedbackPlugin"); }

extern "C" __declspec(dllexport)
PluginObjectType __cdecl GetPluginType() { return(PO_INTERNALS); }

extern "C" __declspec(dllexport)
int __cdecl GetPluginVersion() { return(1); } // InternalsPluginV01 functionality (if you change this return value, you must derive from the appropriate class!)

extern "C" __declspec(dllexport)
PluginObject * __cdecl CreatePluginObject() { return((PluginObject *) new RF2SimFeedbackPlugin); }

extern "C" __declspec(dllexport)
void __cdecl DestroyPluginObject(PluginObject *obj) { delete((RF2SimFeedbackPlugin *)obj); }

TCHAR smName[] = TEXT("Local\\RF2SF");
TCHAR logName[] = TEXT("RF2SimFeedbackPlugin.log");
FILE *logFile;
HANDLE hMapFile;
RF2Data *data;


bool RF2SimFeedbackPlugin::openLogfile() {
	return fopen_s(&logFile, logName, "a") == S_OK;
}

void RF2SimFeedbackPlugin::closeLogFile() {
	fclose(logFile);
}

//Returns the last Win32 error, in string format. Returns an empty string if there is no error.
std::string GetLastErrorAsString()
{
	//Get the error message, if any.
	DWORD errorMessageID = ::GetLastError();
	if (errorMessageID == 0)
		return std::string(); //No error message has been recorded

	LPSTR messageBuffer = nullptr;
	size_t size = FormatMessageA(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
		NULL, errorMessageID, MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), (LPSTR)&messageBuffer, 0, NULL);

	std::string message(messageBuffer, size);

	//Free the buffer.
	LocalFree(messageBuffer);

	return message;
}

bool RF2SimFeedbackPlugin::createSM()
{
	bool retflag = true;

	hMapFile = CreateFileMapping
	(
		INVALID_HANDLE_VALUE,
		NULL,
		FILE_MAP_ALL_ACCESS,
		0,
		sizeof(RF2Data),
		smName
	);

	hMapFile = INVALID_HANDLE_VALUE;
	hMapFile = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, sizeof(RF2Data), smName);
	if (hMapFile == NULL) {
		fprintf(logFile, TEXT("Could not create file mapping object (%d).\n"),
			GetLastError());

		if (GetLastError() == (DWORD)183) 
		{
			hMapFile = OpenFileMapping(FILE_MAP_ALL_ACCESS, FALSE, smName);
			if (hMapFile == NULL) {
				fprintf(logFile, TEXT("Could not open file mapping object (%d).\n"),
					GetLastError());
				return false;
			}
		}
		else 
		{
			return false;
		}
		
	}

	data = (RF2Data*)MapViewOfFile(
		hMapFile, // handle to map object
		FILE_MAP_ALL_ACCESS,  // read/write permission
		0,
		0,
		sizeof(RF2Data));

	if (data == NULL)
	{
		fprintf(logFile, TEXT("Could not map view of file (%d).\n"),
			GetLastError());
		CloseHandle(hMapFile);
		return false;
	}

	memset(data, 0, sizeof(RF2Data));
	//strcpy_s(data->version, RF2DATA_SHARED_MEMORY_VERSION);

	return retflag;
}

void RF2SimFeedbackPlugin::Startup(long version)
{
	

	if (openLogfile()) {

		time_t ltime;
		time(&ltime);
		struct tm* timeinfo = gmtime(&ltime); /* Convert to UTC */
		ltime = mktime(timeinfo); /* Store as unix timestamp */
		fprintf(logFile, "Plugin startup at %li\n", ltime);

		mEnabled = createSM();
		//int msgboxID = MessageBox(NULL, "XXX00", "Loaded", MB_OKCANCEL | MB_DEFBUTTON2);
	}
}


void RF2SimFeedbackPlugin::Shutdown()
{
	UnmapViewOfFile(data);
	CloseHandle(hMapFile);
	closeLogFile();
}


void RF2SimFeedbackPlugin::StartSession()
{
	fprintf(logFile, "Session started\n");
		
}


void RF2SimFeedbackPlugin::EndSession()
{
	fprintf(logFile, "Session stopped\n");
}


void RF2SimFeedbackPlugin::EnterRealtime()
{
	fprintf(logFile, "Relatime entered\n");
	// start up timer every time we enter realtime
	mET = 0.0;
}


void RF2SimFeedbackPlugin::ExitRealtime()
{
	fprintf(logFile, "Relatime exited\n");
}


void RF2SimFeedbackPlugin::UpdateTelemetry(const TelemInfoV01 &info)
{
	if (!mEnabled) return;

	data->time = (float)mET;

	// Compute some auxiliary info based on the above
	TelemVect3 forwardVector = { -info.mOri[0].z, -info.mOri[1].z, -info.mOri[2].z };
	TelemVect3    leftVector = { info.mOri[0].x,  info.mOri[1].x,  info.mOri[2].x };

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

	const double metersPerSec = sqrt((info.mLocalVel.x * info.mLocalVel.x) +
		(info.mLocalVel.y * info.mLocalVel.y) +
		(info.mLocalVel.z * info.mLocalVel.z));
	//fprintf(logFile, "Speed = %.1f KPH, %.1f MPH\n\n", metersPerSec * 3.6, metersPerSec * 2.237);

	data->speed = metersPerSec;

	data->rpm = info.mEngineRPM;


}