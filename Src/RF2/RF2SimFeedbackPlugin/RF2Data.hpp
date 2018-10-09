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
#pragma once

// Our world coordinate system is left-handed, with +y pointing up.
// The local vehicle coordinate system is as follows:
//   +x points out the left side of the car (from the driver's perspective)
//   +y points out the roof
//   +z points out the back of the car
// Rotations are as follows:
//   +x pitches up
//   +y yaws to the right
//   +z rolls to the right
// Note that ISO vehicle coordinates (+x forward, +y right, +z upward) are
// right-handed.  If you are using that system, be sure to negate any rotation
// or torque data because things rotate in the opposite direction.  In other
// words, a -z velocity in rFactor is a +x velocity in ISO, but a -z rotation
// in rFactor is a -x rotation in ISO!!!

#define RF2DATA_SHARED_MEMORY_VERSION "00.01.01"

struct RF2Data {
	//char version[16];	//
	float time;			// time since started      
	float pitch;		// pitch in radians (+ up)
	float roll;			// roll in radians (+ right)
	float accelX;		// local lateral acceleration (+ left) in m/s^2
	float accelY;		// local vertical acceleration (+ up) in m/s^2
	float accelZ;		// local longitudinal acceleration (+ back) in m/s^2
	float rotAccelX;	// local pitch rotational acceleration (+ up) in radians/sec^2
	float rotAccelY;	// local yaw rotational acceleration (+ right) in radians/sec^2
	float rotAccelZ;	// local roll rotational acceleration (+ right) in radians/sec^2
	float speed;		// speed in m/s
	float rpm;			// engine rounds in r/m
};