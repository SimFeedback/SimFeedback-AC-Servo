//ÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜÜ
//İ                                                                         Ş
//İ Module: Header file for internals plugin                                Ş
//İ                                                                         Ş
//İ Description: Interface declarations for internals plugin                Ş
//İ                                                                         Ş
//İ This source code module, and all information, data, and algorithms      Ş
//İ associated with it, are part of isiMotor Technology (tm).               Ş
//İ                 PROPRIETARY AND CONFIDENTIAL                            Ş
//İ Copyright (c) 1996-2007 Image Space Incorporated.  All rights reserved. Ş
//İ                                                                         Ş
//İ Change history:                                                         Ş
//İ   tag.2005.11.29: created                                               Ş
//İ                                                                         Ş
//ßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßßß

#ifndef _INTERNALS_PLUGIN_HPP_
#define _INTERNALS_PLUGIN_HPP_

#include "RFPluginObjects.hpp"

// change this variable whenever a change is made to the plugin interfaces
// use m_uVersion (returned by GetVersion() for version control instead //#define INTERNALS_PLUGIN_VERSION (0.0f)


//ÚÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄ¿
//³ Structs to retrieve internal information (e.g. telemetry info)         ³
//ÀÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÙ

struct TelemVect3
{
  float x, y, z;

  void Set( const float a, const float b, const float c )  { x = a; y = b; z = c; }
};


struct TelemWheel
{
  float mRotation;               // radians/sec
  float mSuspensionDeflection;   // meters
  float mRideHeight;             // meters
  float mTireLoad;               // Newtons
  float mLateralForce;           // Newtons
  float mGripFract;              // an approximation of what fraction of the contact patch is sliding
  float mBrakeTemp;              // Celsius
  float mPressure;               // kPa
  float mTemperature[3];         // Celsius, left/center/right (not to be confused with inside/center/outside!)
};

struct TelemWheelV2 : public TelemWheel
{
  float mWear;                   // wear (0.0-1.0, fraction of maximum) ... this is not necessarily proportional with grip loss
  char  mTerrainName[16];        // the material prefixes from the TDF file
  unsigned char mSurfaceType;    // 0=dry, 1=wet, 2=grass, 3=dirt, 4=gravel, 5=rumblestrip
  bool mFlat;                    // whether tire is flat
  bool mDetached;                // whether wheel is detached

  // Future use
  unsigned char mExpansion[32];
};


// Our world coordinate system is left-handed, with +y pointing up.
// The local vehicle coordinate system is as follows:
//   +x points out the left side of the car (from the driver's perspective)
//   +y points out the roof
//   +z points out the back of the car
// Rotations are as follows:
//   +x pitches up
//   +y yaws to the right
//   +z rolls to the right

struct TelemInfoBase
{
  // Time
  float mDeltaTime;              // time since last update (seconds)
  long mLapNumber;               // current lap number
  float mLapStartET;             // time this lap was started
  char mVehicleName[64];         // current vehicle name
  char mTrackName[64];           // current track name

  // Position and derivatives
  TelemVect3 mPos;               // world position in meters
  TelemVect3 mLocalVel;          // velocity (meters/sec) in local vehicle coordinates
  TelemVect3 mLocalAccel;        // acceleration (meters/sec^2) in local vehicle coordinates

  // Orientation and derivatives
  TelemVect3 mOriX;              // top row of orientation matrix (also converts local vehicle vectors into world X using dot product)
  TelemVect3 mOriY;              // mid row of orientation matrix (also converts local vehicle vectors into world Y using dot product)
  TelemVect3 mOriZ;              // bot row of orientation matrix (also converts local vehicle vectors into world Z using dot product)
  TelemVect3 mLocalRot;          // rotation (radians/sec) in local vehicle coordinates
  TelemVect3 mLocalRotAccel;     // rotational acceleration (radians/sec^2) in local vehicle coordinates

  // Vehicle status
  long mGear;                    // -1=reverse, 0=neutral, 1+=forward gears
  float mEngineRPM;              // engine RPM
  float mEngineWaterTemp;        // Celsius
  float mEngineOilTemp;          // Celsius
  float mClutchRPM;              // clutch RPM

  // Driver input
  float mUnfilteredThrottle;     // ranges  0.0-1.0
  float mUnfilteredBrake;        // ranges  0.0-1.0
  float mUnfilteredSteering;     // ranges -1.0-1.0 (left to right)
  float mUnfilteredClutch;       // ranges  0.0-1.0

  // Misc
  float mSteeringArmForce;       // force on steering arms
};

struct TelemInfo : public TelemInfoBase  // re-arranged for expansion, but backwards-compatible
{
  TelemWheel mWheel[4];          // wheel info (front left, front right, rear left, rear right)
};

struct TelemInfoV2 : public TelemInfoBase // for noobs: TelemInfoV2 contains everything in TelemInfoBase, plus the following:
{
  // state/damage info
  float mFuel;                   // amount of fuel (liters)
  float mEngineMaxRPM;           // rev limit
  unsigned char mScheduledStops; // number of scheduled pitstops
  bool  mOverheating;            // whether overheating icon is shown
  bool  mDetached;               // whether any parts (besides wheels) have been detached
  unsigned char mDentSeverity[8];// dent severity at 8 locations around the car (0=none, 1=some, 2=more)
  float mLastImpactET;           // time of last impact
  float mLastImpactMagnitude;    // magnitude of last impact
  TelemVect3 mLastImpactPos;     // location of last impact

  // Automobilista
  unsigned char mDrsState;
  bool mDrsActive;
  unsigned char mPushToPassState;
  bool mGearboxGrinding;
  long mGearboxDamage;
  float mLapDist;
  unsigned char mEngineBoostMapping;
  unsigned char mStartLight;

  // Future use
  unsigned char mPadding1;
  unsigned char mPadding2;
  unsigned char mPadding3;
  unsigned char mExpansion[44];

  // keeping this at the end of the structure to make it easier to replace in future versions
  TelemWheelV2 mWheel[4];        // wheel info (front left, front right, rear left, rear right)
};


struct GraphicsInfo              // may be expanded in the future to provide interfaces for drawing onscreen
{
  TelemVect3 mCamPos;            // camera position
  TelemVect3 mCamOri;            // camera orientation
  HWND mHWND;                    // app handle
};

struct GraphicsInfoV2 : public GraphicsInfo // for noobs: GraphicsInfoV2 contains everything in GraphicsInfo, plus the following:
{
  float mAmbientRed;
  float mAmbientGreen;
  float mAmbientBlue;
};


struct VehicleScoringInfo
{
  char mDriverName[32];          // driver name
  char mVehicleName[64];         // vehicle name
  short mTotalLaps;              // laps completed
  signed char mSector;           // 0=sector3, 1=sector1, 2=sector2 (don't ask why)
  signed char mFinishStatus;     // 0=none, 1=finished, 2=dnf, 3=dq
  float mLapDist;                // current distance around track
  float mPathLateral;            // lateral position with respect to *very approximate* "center" path
  float mTrackEdge;              // track edge (w.r.t. "center" path) on same side of track as vehicle

  float mBestSector1;            // best sector 1
  float mBestSector2;            // best sector 2 (plus sector 1)
  float mBestLapTime;            // best lap time
  float mLastSector1;            // last sector 1
  float mLastSector2;            // last sector 2 (plus sector 1)
  float mLastLapTime;            // last lap time
  float mCurSector1;             // current sector 1 if valid
  float mCurSector2;             // current sector 2 (plus sector 1) if valid
  // no current laptime because it instantly becomes "last"

  short mNumPitstops;            // number of pitstops made
  short mNumPenalties;           // number of outstanding penalties
};


struct VehicleScoringInfoV2 : public VehicleScoringInfo // for noobs: VehicleScoringInfoV2 contains everything in VehicleScoringInfo, plus the following:
{
  bool mIsPlayer;                // is this the player's vehicle
  signed char mControl;          // who's in control: -1=nobody (shouldn't get this), 0=local player, 1=local AI, 2=remote, 3=replay (shouldn't get this)
  bool mInPits;                  // between pit entrance and pit exit (not always accurate for remote vehicles)
  unsigned char mPlace;          // 1-based position
  char mVehicleClass[32];        // vehicle class

  // Dash Indicators
  float mTimeBehindNext;         // time behind vehicle in next higher place
  long mLapsBehindNext;          // laps behind vehicle in next higher place
  float mTimeBehindLeader;       // time behind leader
  long mLapsBehindLeader;        // laps behind leader
  float mLapStartET;             // time this lap was started

  // Position and derivatives
  TelemVect3 mPos;               // world position in meters
  TelemVect3 mLocalVel;          // velocity (meters/sec) in local vehicle coordinates
  TelemVect3 mLocalAccel;        // acceleration (meters/sec^2) in local vehicle coordinates

  // Orientation and derivatives
  TelemVect3 mOriX;              // top row of orientation matrix (also converts local vehicle vectors into world X using dot product)
  TelemVect3 mOriY;              // mid row of orientation matrix (also converts local vehicle vectors into world Y using dot product)
  TelemVect3 mOriZ;              // bot row of orientation matrix (also converts local vehicle vectors into world Z using dot product)
  TelemVect3 mLocalRot;          // rotation (radians/sec) in local vehicle coordinates
  TelemVect3 mLocalRotAccel;     // rotational acceleration (radians/sec^2) in local vehicle coordinates

  // Future use
  unsigned char mExpansion[128];
};


struct ScoringInfoBase
{
  char mTrackName[64];           // current track name
  long mSession;                 // current session
  float mCurrentET;              // current time
  float mEndET;                  // ending time
  long  mMaxLaps;                // maximum laps
  float mLapDist;                // distance around track
  char *mResultsStream;          // results stream additions since last update (newline-delimited and NULL-terminated)

  long mNumVehicles;             // current number of vehicles
};

struct ScoringInfo : public ScoringInfoBase  // re-arranged for expansion, but backwards-compatible
{
  VehicleScoringInfo *mVehicle;  // array of vehicle scoring info's
};

struct ScoringInfoV2 : public ScoringInfoBase // for noobs: ScoringInfoV2 contains everything in ScoringInfoBase, plus the following:
{
  // Game phases:
  // 0 Before session has begun
  // 1 Reconnaissance laps (race only)
  // 2 Grid walk-through (race only)
  // 3 Formation lap (race only)
  // 4 Starting-light countdown has begun (race only)
  // 5 Green flag
  // 6 Full course yellow / safety car
  // 7 Session stopped
  // 8 Session over
  unsigned char mGamePhase;   

  // Yellow flag states (applies to full-course only)
  // -1 Invalid
  //  0 None
  //  1 Pending
  //  2 Pits closed
  //  3 Pit lead lap
  //  4 Pits open
  //  5 Last lap
  //  6 Resume
  //  7 Race halt (not currently used)
  signed char mYellowFlagState;

  signed char mSectorFlag[3];      // whether there are any local yellows at the moment in each sector (not sure if sector 0 is first or last, so test)
  unsigned char mStartLight;       // start light frame (number depends on track)
  unsigned char mNumRedLights;     // number of red lights in start sequence
  bool mInRealtime;                // in realtime as opposed to at the monitor
  char mPlayerName[32];            // player name (including possible multiplayer override)
  char mPlrFileName[64];           // may be encoded to be a legal filename

  // weather
  float mDarkCloud;                // cloud darkness? 0.0-1.0
  float mRaining;                  // raining severity 0.0-1.0
  float mAmbientTemp;              // temperature (Celsius)
  float mTrackTemp;                // temperature (Celsius)
  TelemVect3 mWind;                // wind speed
  float mOnPathWetness;            // on main path 0.0-1.0
  float mOffPathWetness;           // on main path 0.0-1.0

  // Automobilista
  unsigned char mPadding[56];
  long mRaceLaps;

  // Future use
  unsigned char mExpansion[196];

  // keeping this at the end of the structure to make it easier to replace in future versions
  VehicleScoringInfoV2 *mVehicle;  // array of vehicle scoring info's
};


struct CommentaryRequestInfo
{
  char mName[32];                  // one of the event names in the commentary INI file
  double mInput1;                  // first value to pass in (if any)
  double mInput2;                  // first value to pass in (if any)
  double mInput3;                  // first value to pass in (if any)
  bool mSkipChecks;                // ignores commentary detail and random probability of event

  // constructor (for noobs, this just helps make sure everything is initialized to something reasonable)
  CommentaryRequestInfo()          { mName[0] = 0; mInput1 = 0.0; mInput2 = 0.0; mInput3 = 0.0; mSkipChecks = false; }
};


//ÚÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄ¿
//³ Plugin classes used to access internals                                ³
//ÀÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÙ

// Note: use class InternalsPlugin   for GetVersion()==1, or
//       use class InternalsPluginV2 for GetVersion()==2, or
//       use class InternalsPluginV3 for GetVersion()==3
class InternalsPlugin : public PluginObject
{
 public:

  // General internals methods
  InternalsPlugin() {}
  virtual ~InternalsPlugin() {}

  virtual void Startup() {}                                    // game startup
  virtual void Shutdown() {}                                   // game shutdown

  virtual void EnterRealtime() {}                              // entering realtime (where the vehicle can be driven)
  virtual void ExitRealtime() {}                               // exiting realtime

  // GAME OUTPUT
  virtual bool WantsTelemetryUpdates() { return( false ); }    // whether we want telemetry updates
  virtual void UpdateTelemetry( const TelemInfo &info ) {}     // update plugin with telemetry info

  virtual bool WantsGraphicsUpdates() { return( false ); }     // whether we want graphics updates
  virtual void UpdateGraphics( const GraphicsInfo &info ) {}   // update plugin with graphics info

  // GAME INPUT
  virtual bool HasHardwareInputs() { return( false ); }        // whether plugin has hardware plugins
  virtual void UpdateHardware( const float fDT ) {}            // update the hardware with the time between frames
  virtual void EnableHardware() {}                             // message from game to enable hardware
  virtual void DisableHardware() {}                            // message from game to disable hardware

  // See if the plugin wants to take over a hardware control.  If the plugin takes over the
  // control, this method returns true and sets the value of the float pointed to by the
  // second arg.  Otherwise, it returns false and leaves the float unmodified.
  virtual bool CheckHWControl( const char * const controlName, float &fRetVal ) { return false; }

  virtual bool ForceFeedback( float &forceValue ) { return( false ); } // alternate force feedback computation - return true if editing the value
};


class InternalsPluginV2 : public InternalsPlugin // for noobs: InternalsPluginV2 contains everything in InternalsPlugin, plus the following:
{
 public:

  // SCORING OUTPUT
  virtual bool WantsScoringUpdates() { return( false ); }      // whether we want scoring updates
  virtual void UpdateScoring( const ScoringInfo &info ) {}     // update plugin with scoring info (approximately once per second)
};


class InternalsPluginV3 : public InternalsPluginV2 // for noobs: InternalsPluginV3 contains everything in InternalsPluginV2 (and InternalsPlugin), plus the following:
{
 public:

  // SESSION NOTIFICATIONS
  virtual void StartSession() {}                               // session started
  virtual void EndSession() {}                                 // session ended

  // GAME OUTPUT
  virtual void UpdateTelemetry( const TelemInfoV2 &info ) {}   // update plugin with telemetry info

  // GRAPHICS OUTPUT
  virtual void UpdateGraphics( const GraphicsInfoV2 &info ) {} // update plugin with graphics info

  // SCORING OUTPUT
  virtual void UpdateScoring( const ScoringInfoV2 &info ) {}   // update plugin with scoring info (approximately once per second)

  // COMMENTARY INPUT
  virtual bool RequestCommentary( CommentaryRequestInfo &info ) { return( false ); } // to use our commentary event system, fill in data and return true
};

//ÚÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄ¿
//ÀÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÙ

#endif // _INTERNALS_PLUGIN_HPP_

