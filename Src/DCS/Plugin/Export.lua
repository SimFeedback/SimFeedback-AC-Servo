--
-- Copyright (c) 2018 Rausch IT
--
-- Permission is hereby granted, free of charge, to any person obtaining a copy 
-- of this software and associated documentation files (the "Software"), to deal
-- in the Software without restriction, including without limitation the rights 
-- to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
-- copies of the Software, and to permit persons to whom the Software is 
-- furnished to do so, subject to the following conditions:
--
-- The above copyright notice and this permission notice shall be included in 
-- all copies or substantial portions of the Software.
--
-- THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
-- IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
-- FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
-- THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
-- LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
-- OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
-- THE SOFTWARE.
--
--
-- SimFeedback DCS Lua script to export position and orientation data

-- SimFeedbackExporter {}

--local lfs = require('lfs')
--LUA_PATH = "?;?.lua;"..lfs.currentdir().."/Scripts/?.lua"
--require 'Vector'

local udpServer = nil

local t0 = 0

local PrevExport                    = {}
PrevExport.LuaExportStart           = LuaExportStart
PrevExport.LuaExportStop            = LuaExportStop
PrevExport.LuaExportBeforeNextFrame = LuaExportBeforeNextFrame
PrevExport.LuaExportAfterNextFrame  = LuaExportAfterNextFrame

local default_output_file = nil

function LuaExportStart()
	default_output_file = io.open(lfs.writedir().."/Logs/SimFeedback.log", "w")

	local version = LoGetVersionInfo() 
	if version and default_output_file then
		default_output_file:write("ProductName: "..version.ProductName..'\n')
		default_output_file:write(string.format("FileVersion: %d.%d.%d.%d\n",
												version.FileVersion[1],
 												version.FileVersion[2],
												version.FileVersion[3],
 												version.FileVersion[4]))
 		default_output_file:write(string.format("ProductVersion: %d.%d.%d.%d\n",
 												version.ProductVersion[1],
 												version.ProductVersion[2],
 												version.ProductVersion[3], 
												version.ProductVersion[4]))
	end
	
--  Socket
	package.path  = package.path..";"..lfs.currentdir().."/LuaSocket/?.lua"
	package.cpath = package.cpath..";"..lfs.currentdir().."/LuaSocket/?.dll"
	socket = require("socket")
	host = host or "localhost"
	port = port or 6666
	
	udpServer = socket.udp()
	udpServer:setoption('reuseaddr',true)
	udpServer:setpeername(host, port)
end


function LuaExportBeforeNextFrame()

end


function LuaExportAfterNextFrame()
---[[
	local curTime = LoGetModelTime()
	if curTime >= t0 then
		-- runs 100 times per second
		t0 = curTime + .01

		local pitch, roll, yaw = LoGetADIPitchBankYaw()
		-- local velocityvector = LoGetVectorVelocity()
		-- local RotationalVelocity = LoGetAngularVelocity()
		-- local VerticalVelocity = LoGetVerticalVelocity()		
		
		if udpServer then
			socket.try(udpServer:send(string.format("%.3f;%.3f;%.3f;%.3f", t0, 57.3*pitch, 57.3*roll, 57.3*yaw)))
		end

		--default_output_file:write(string.format("%.3f%.2f%.2f%.2f\n", t0, pitch, bank, yaw))
	end
--]]
end

--[[
function LuaExportActivityNextEvent(t)
	local t0 = t

    if LoGetPlayerPlaneId() > 0 then
        local data = LoGetSelfData()
        
        local yaw   = 2.0 * math.pi - data.Heading 
        local pitch = data.Pitch
        local roll  = data.Bank
		
		if udpServer then
			socket.try(udpServer:send(string.format("%.3f;%.3f;%.3f;%.3f", t0, 57.3*pitch, 57.3*roll, 57.3*yaw)))
		end
		
	end
	-- 100 samples/sec
    t0 = t0 + .01
	return t0	
end
--]]

function LuaExportStop()
-- Works once just after mission stop.
	if PrevExport.LuaExportStop then
		PrevExport.LuaExportStop()
	end
	
	if default_output_file then
		default_output_file:close()
		default_output_file = nil
	end
	
	if udpServer then
		udpServer:close()
	end
end