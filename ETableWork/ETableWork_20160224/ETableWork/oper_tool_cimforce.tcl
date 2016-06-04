######################################################################
#    OPER _ TOOL . T C L 
######################################################################
# EVENT HANDLER SECTION 
#
# Revision History
# ----------------
#    Rev  Date        Who    PR	  Reason
#
#    00   1Apr2004   marty       Initial
#    01   2Apr2004   marty       Handle the case that operation count is larger than 15
#    02   27Apr2004 marty       Get the value of depth of cut based on the operation type
#    03   08Jun2004 marty       disable output part material
#    04   28Jun2004 marty       use mom_tool_corner1_radius instead of tool radius.
#    05   08Nov2006 marty      Add a NC File (group name) column and change the shopdoc layout
#    06   20Nov2006 marty      use mom_global_cut_depth instead of mom_range_1_depth_per_cut for Cavity Milling
#    07   28Nov2006 marty      Specify picture height instead picture scale so that picture can fit all in Excel window.
#    08   18Apr2007 marty      Modify template file and use different cycling syntax. Eliminate the use of program_list
#    09   01May2007 marty      Call shopdoc_partinfo.dll to get the stock size and maximum z. 1.1	伸出長和首下長設定為PART最高點與Z_min的差值加上1mm
#                                                 切削模式 use mom_oper_method instead of mom_oper_type. Set naming rule for 模號 and 件號
#    10   20Jul2009 marty      support two levels of program group (Work->File Name->Operation)
#
# issues:
#    1. stock: mom_stock_part or mom_stock_floor
#
######################################################################
#_______________________________________________________________________________
# Here you should define any global variables that will be used in any one
# of the event handler.
#_______________________________________________________________________________
#source [MOM_ask_env_var UGII_CAM_DEBUG_DIR]mom_review.tcl

#MOM_set_debug_mode ON
  #MOM_set_debug_mode OFF
  set mom_sys_commentary_output              "OFF"
  set mom_sys_list_output                    "OFF"
  set mom_sys_header_output                     "OFF"
  set mom_sys_group_output                   "OFF"
  set mom_sys_warning_output                    "OFF"



set mom_source_directory [MOM_ask_env_var UGII_CAM_SHOP_DOC_DIR]
source "[MOM_ask_env_var UGII_CAM_SHOP_DOC_DIR]\\shopdoc_empty_handlers.tcl" 


set simple_tcl "$mom_source_directory\\mill3ax_Cimforce.tcl"
set simple_def "$mom_source_directory\\mill3ax_Cimforce.def"

set template "$mom_source_directory\\oper_tool_cimforce.tpl"

set tojpg_dll "$mom_source_directory"
append tojpg_dll "plot_jpg.dll"

set takepic_dll "$mom_source_directory"
append takepic_dll "TakePic.dll"

#set ExportSTL_dll "$mom_source_directory"
#append ExportSTL_dll "ExportSTL.dll"

set IS_FIRST 0

#set LOG_FILE "E:\\shopdocLog.txt"
#set HOLDER_DATA "D:\\SHopDoc_TEST\\UserID\\WorkNum\\Assignment\\WORK_CNC3_BH_T\\SHOP_DOC"

# This procedure creates a part documentation.
#=============================================================================
proc MOM_Start_Part_Documentation {} {
#=============================================================================
	#global E_TABLE
	#set output_log_file [open $E_TABLE w]
	
	#global temp_dir
	#global env
	#set temp_dir $env(UGII_USER_DIR)
	#puts $output_log_file $temp_dir
	#close $output_log_file
	
	#set output_log_file [open "D:\\oper_tool_cimforce.csv" w]
	#puts $output_log_file "work_name,nc_name,operation_name,cutting_time,tool_name,tool_number,feed,XY_Stock,operation_gif,RPM,depth_per_cut,operation_type,MAX_Z,Clearance_plane,H_code,Tool_dia,Tool_radius,Tool_length,Z_Stock,X/Y_stepover,stepover_type"
	#close $output_log_file
}

#=============================================================================
proc MOM_Part_Documentation {} {
#=============================================================================

	#set output_log_file [open $LOG_FILE a]
	#puts $output_log_file "MOM_Part_Documentation INPUT"
	#close $output_log_file
	
	#global E_TABLE
	#set output_log_file [open $E_TABLE w]
	#puts $output_log_file ""
	#close $output_log_file
	#close $output_log_file

	global mom_logname
	global mom_part_name
	
	global current_date
	global part_name
	global mold_number
	global case_number
	global DATA_START_TIMES
	set DATA_START_TIMES 0
	
	set current_date ""
	set part_name ""
	set mold_number ""
	set case_number ""
	
	catch {set current_date [clock format [clock seconds] -format "%Y/%m/%d"]}
	catch {set part_name [file rootname [file tail $mom_part_name]]}
	catch {set mold_number [string range $part_name 0 4]}
    catch {set case_number [string range $part_name 5 end]}
	
	global part_folder	
	set find_index 0
	set part_name_length 0

	set find_index [string last "\\" $mom_part_name]
	set part_name_length [string length $mom_part_name]
	set part_folder [string range $mom_part_name 0 [expr $find_index-1]]

	
	global E_TABLE
	global HOLDER_DATA

	set chk_path 0
	
	set E_TABLE "$part_folder\\SHOP_DOC_TEMP\\shopdoc.dat"
	set HOLDER_DATA "$part_folder\\SHOP_DOC_TEMP"
		
	catch {
		set output_log_file [open $E_TABLE w]
		puts $output_log_file ""
		close $output_log_file
		set chk_path 1
	}
	
	if {$chk_path == 0} {
		set E_TABLE "$part_folder\\SHOP_DOC_TEMP\\shopdoc.dat"
		set HOLDER_DATA "$part_folder"
		set output_log_file [open $E_TABLE w]
		puts $output_log_file ""
		close $output_log_file
	}
	
	
	
	
#	set output_msg [open "D:\\11111.txt" w]
#	puts $output_msg "part_folder"
#	puts $output_msg "$mom_part_name"
#	puts $output_msg "$part_folder"
#	close $output_msg
	
	#close $output_log_file
	
    global template
    MOM_do_template_file $template
	
		#set output_log_file [open $LOG_FILE w]
	#puts $output_log_file "template INPUT"
	#close $output_log_file
	
	#set url "http://tw.yahoo.com/?A=1"
	#exec "C:/Program Files/Internet Explorer/iexplore.exe" $url &
	
	global E_TABLE
	set output_log_file [open $E_TABLE a]
	catch {puts $output_log_file "\[PROGRAMS_END\]"}
	close $output_log_file
	
	#set output_log_file [open $E_TABLE a]
	#puts $output_log_file "MOM_Part_Documentation OUTPUT"
	#close $output_log_file
	
	
#	set isstl_data 0
	
#	catch {
#		set fd [open "$part_folder\\isstl.dat" r]
#		set isstl_data [read $fd]
#		puts $isstl_data
#		close $fd
#	}
	
#	global ExportSTL_dll
#	if {[expr $isstl_data] == 1} {
		#file delete -force "$part_folder\\isstl.dat"
#		catch {MOM_run_user_function $ExportSTL_dll "Main"}
#	}	
	
	global tojpg_dll
	global takepic_dll

	#抓取裝夾圖
	#catch {MOM_run_user_function $tojpg_dll "ufusr"}
	#catch {MOM_run_user_function $takepic_dll "Main"}


}

#=============================================================================
proc MOM_End_Part_Documentation {} {
#=============================================================================
	#set output_log_file [open $::E_TABLE a]
	#puts $output_log_file "MOM_End_Part_Documentation"
	#close $output_log_file
}

#===============================================================================
# Setup
#===============================================================================
proc MOM_SETUP_HDR {} {
	#set output_log_file [open $::E_TABLE a]
	#puts $output_log_file "MOM_SETUP_HDR"
	#close $output_log_file
}

#=============================================================================
proc MOM_SETUP_BODY {} {
#=============================================================================
	#set output_log_file [open $::E_TABLE a]
	#puts $output_log_file "MOM_SETUP_BODY"
	#close $output_log_file
}


#=============================================================================
proc MOM_SETUP_FTR {} {
#=============================================================================
	#set output_log_file [open $::E_TABLE a]
	#puts $output_log_file "MOM_SETUP_FTR"
	#close $output_log_file
}

#=============================================================================
proc  MOM_MEMBERS_HDR {} {
#=============================================================================
    
	#set output_log_file [open $::E_TABLE a]
	#puts $output_log_file "MOM_MEMBERS_HDR\=mom_object_type_name \: top_level_parent\=mom_oper_program"
	#close $output_log_file

	global work_name
	global nc_name
	global operation_name
	global cutting_time
	global tool_name
	global tool_number
	global feed
	global XY_Stock
	global operation_gif
	global RPM
	global depth_of_cut
	global operation_type
	global MAX_Z
	global clearance_plane
	global H_code
	global tool_dia
	global tool_radius
	global tool_length
	global Z_Stock
	global stepover
	global stepover_type
	global mom_stock_part_use

	#set work_name ""
	set nc_name ""
	set operation_name ""
	set cutting_time ""
	set tool_name ""
	set tool_number ""
	set feed ""
	set XY_Stock ""
	set operation_gif ""
	set RPM ""
	set depth_of_cut ""
	set operation_type ""
	set MAX_Z ""
	set clearance_plane ""
	set H_code ""
	set tool_dia ""
	set tool_radius ""
	set tool_length ""
	set Z_Stock 0
	set stepover ""
	set stepover_type ""
	#set mom_stock_part_use 0
	
	global xy_side_stock
	global z_floor_stock
	
	set xy_side_stock ""
	set z_floor_stock ""
	

	
	global mom_oper_program
	global mom_operation_name
	global mom_operation_type
	global mom_toolpath_cutting_time
	global mom_toolpath_cutting_length
	
	global oper_program
	global operation_type
	global cutting_time
	global cutting_length
	
	catch { set oper_program $mom_oper_program }
	catch { set operation_type $mom_operation_type }
	catch {	set nc_name $mom_oper_program}
	catch {	set operation_name $mom_operation_name}
	catch {	set cutting_time $mom_toolpath_cutting_time}
	catch {	set cutting_length $mom_toolpath_cutting_length}
	
	global mom_common_depth_per_cut_type
	global mom_global_cut_depth_source
	global mom_global_cut_depth
	global mom_scallop_common_depth_per_cut
	
	set depth_of_cut ""
	#set depth_per_cut_type -1
	catch {	
		set depth_per_cut_type $mom_common_depth_per_cut_type 
		
		if {$depth_per_cut_type == 0} {#Constant
			
			set unit_type 0
			catch {
				set unit_type $mom_global_cut_depth_source
			}
			
			if {$unit_type == 4} {
				catch {
					set depth_of_cut "[format %.3f $mom_global_cut_depth] %Tool"
				}
			} else {
				catch {
					set depth_of_cut "[format %.3f $mom_global_cut_depth] mm"
				}
			}
		} elseif {$depth_per_cut_type == 1} {#Scallop
			catch {	
				set depth_of_cut "$mom_scallop_common_depth_per_cut"
			}
		}	
	}

	global mom_group_name
	set work_name $mom_group_name

	global DATA_START_TIMES
	if { $DATA_START_TIMES==0 } {
		set DATA_START_TIMES 1
		global E_TABLE
		set output_log_file [open $E_TABLE a]
		catch {puts $output_log_file "\[DATA_START\]"}
		catch {puts $output_log_file "\{ \"name\":\"FIXTURE_PNG\", \"value\":\"$work_name.png\" \},"}
		catch {puts $output_log_file "\{ \"name\":\"FIXTURE_PATH\", \"value\":\"SHOP_DOC/$work_name.png\" \},"}
		catch {puts $output_log_file "\[DATA_END\]"}
		catch {puts $output_log_file "\[PROGRAMS_START\]"}
		close $output_log_file
	}
	


}

#=============================================================================
proc MOM_MEMBERS_FTR { } {
#=============================================================================
    
	#set output_log_file [open $::E_TABLE a]
	#puts $output_log_file "MOM_MEMBERS_FTR=mom_object_type_name"
	#close $output_log_file
}

proc MOM_PROGRAMVIEW_HDR {} {

	#set output_log_file [open $::E_TABLE a]
	#puts $output_log_file "MOM_PROGRAMVIEW_HDR"
	#close $output_log_file
}

proc MOM_PROGRAMVIEW_FTR {} {

	#set output_log_file [open $::E_TABLE a]
	#puts $output_log_file "MOM_PROGRAMVIEW_FTR"
	#close $output_log_file
}

proc MOM_PROGRAM_BODY {} {

	global mom_object_name
	global mom_member_count

	#set output_log_file [open $::E_TABLE a]
	#puts $output_log_file "MOM_PROGRAM_BODY=mom_object_name : $mom_object_name=mom_member_count : $mom_member_count"
	#close $output_log_file

	global mom_member_nest_level
	global mom_group_name
	global work_name
	global IS_FIRST
	
	global mom_logname
	global mom_part_name
	
	global current_date
	global part_name
	global mold_number
	global case_number
	global DATA_START_TIMES
	
	if {$mom_member_count > 1 && $mom_member_nest_level ==0} {
		set work_name $mom_group_name
		
		if {$DATA_START_TIMES == 0 && $work_name != "NC_PROGRAM"} {
			set DATA_START_TIMES 1
			
			global E_TABLE
			set output_log_file [open $E_TABLE a]
			catch {puts $output_log_file "\[DATA_START\]"}
			catch {puts $output_log_file "\{ \"name\":\"FIXTURE_PNG\", \"value\":\"$work_name.png\" \},"}
			catch {puts $output_log_file "\{ \"name\":\"FIXTURE_PATH\", \"value\":\"SHOP_DOC/$work_name.png\" \},"}
			catch {puts $output_log_file "\[DATA_END\]"}
			catch {puts $output_log_file "\[PROGRAMS_START\]"}
			close $output_log_file

			#global E_TABLE
			#set output_log_file [open $E_TABLE a]

			#catch {puts $output_log_file "\[DATA_START\]"}
			#catch {puts $output_log_file "\{ \"name\":\"FIXTURE_PNG\", \"value\":\"$work_name.png\" \},"}
			#catch {puts $output_log_file "\{ \"name\":\"FIXTURE_PATH\", \"value\":\"SHOP_DOC/$work_name.png\" \},"}
			#catch {puts $output_log_file "\[DATA_END\]"}
			#catch {puts $output_log_file "\[PROGRAMS_START\]"}
			#close $output_log_file
		}
	}
}

proc MOM_PROGRAM_FTR {} {
	#set output_log_file [open $::E_TABLE a]
	#puts $output_log_file "MOM_PROGRAM_FTR"
	#close $output_log_file
}

#===============================================================================
# Operation
#===============================================================================
proc MOM_OPER_HDR {} {
	#set output_log_file [open $::E_TABLE a]
	#puts $output_log_file "MOM_OPER_HDR"
	#close $output_log_file
}

proc MOM_OPER_FTR {} {
	#set output_log_file [open $::E_TABLE a]
	#puts $output_log_file "MOM_OPER_FTR"
	#close $output_log_file
}

proc MOM_OPER_BODY {} {

	#set output_log_file [open $::E_TABLE a]
	#puts $output_log_file "MOM_OPER_BODY=====MOM_TOOL_BODY"
	
	#puts $output_log_file "aaaaaaaa"
	#puts $output_log_file "mom_stock_floor \: $mom_stock_floor"

	global mom_stock_part
	#catch { set $mom_stock_part [format %.3f 0] }
	global mom_stock_floor
	#catch { set $mom_stock_floor [format %.3f 0] }
	#global mom_global_cut_depth
	global mom_stepover_distance_source
	global mom_stepover_distance
	global mom_stepover_scallop
	global mom_stepover_percent
	global mom_stepover_type
	global mom_feed_cut_value
	global mom_spindle_rpm
	global mom_operation_type
	global mom_clearance_plane_point
	global mom_clearance_plane_normal
	global mom_stepover_variable_values
	global mom_msys_origin
	global mom_oper_program
	global mom_toolpath_cutting_time
	global mom_toolpath_cutting_length
	global mom_stock_part_use
	#puts $output_log_file "mom_stock_part \= $mom_stock_part"
	#puts $output_log_file "mom_stock_floor \: $mom_stock_floor"
	#puts $output_log_file "mom_stock_part_use \: $mom_stock_part_use"
	global XY_Stock
	global Z_Stock
	global feed
	global RPM
	global stepover_type
	global clearance_plane
	global stepover
	#global depth_of_cut
	global operation_type
	global oper_program
	global cutting_time
	global cutting_length
	
	# 20151110 Stewart
	#puts $output_log_file "TEST"
	global floor_as_side
	catch { set floor_as_side 0 }
	catch { set floor_as_side $mom_stock_part_use }
	
	catch { set oper_program $mom_oper_program }
	catch { set XY_Stock 0.000 }
	catch {	set XY_Stock [format %.3f $mom_stock_part] }
	catch { set Z_Stock 0.000 }
	catch {	
		if {$floor_as_side == 1 } {
			#puts $output_log_file "if mom_stock_part_use \= 1"
			catch {	set Z_Stock [format %.3f $mom_stock_part] 
			#puts $output_log_file "Z_Stock \: $Z_Stock"
			}
		} else {
			#puts $output_log_file "if mom_stock_part_use \!\= 1"
			catch {	set Z_Stock [format %.3f $mom_stock_floor] 
			#puts $output_log_file "Z_Stock \: $Z_Stock"
			}
		}
	}
	# reset global variables
	catch {	set mom_stock_part_use 0 }
	catch {	set mom_stock_floor 0 }
	catch {	set mom_stock_part 0 }
	#puts $output_log_file "TEST3"
	# 20151110 Stewart

	#catch {	set Z_Stock [format %.3f $mom_stock_floor] }
	catch {	set feed [format %.3f $mom_feed_cut_value] }
	catch {	set RPM [format %.3f $mom_spindle_rpm] }
	catch {	set operation_type $mom_operation_type }
	#unset mom_stock_part
	#unset mom_stock_floor
	#close $output_log_file
	
	catch {	set cutting_time $mom_toolpath_cutting_time}
	catch {	set cutting_length $mom_toolpath_cutting_length}
	
	#set clear_str ""
	#append clear_str [format %.3f $mom_clearance_plane_point(0)]
	#append clear_str "_"
	#append clear_str [format %.3f $mom_clearance_plane_point(1)]
	#append clear_str "_"
	#append clear_str [format %.3f $mom_clearance_plane_point(2)]
	
	#catch {	set clearance_plane $clear_str}
	
	#Abs(-1)
	global normal_dir
	
	set clearance_plane ""
	catch {	
		if {$mom_clearance_plane_normal(0) == 1 || $mom_clearance_plane_normal(0) == -1} {
			set normal_dir 0
			catch {	
				set clearance_plane [expr [format %.3f $mom_msys_origin(0)]-[format %.3f $mom_clearance_plane_point(0)]]
			}
		} elseif {$mom_clearance_plane_normal(1) == 1 || $mom_clearance_plane_normal(1) == -1} {
			set normal_dir 1
			catch {	
				set clearance_plane [expr [format %.3f $mom_msys_origin(1)]-[format %.3f $mom_clearance_plane_point(1)]]
			}
		} elseif {$mom_clearance_plane_normal(2) == 1 || $mom_clearance_plane_normal(2) == -1} {
			set normal_dir 2
			catch {	
				set clearance_plane [expr [format %.3f $mom_msys_origin(2)]-[format %.3f $mom_clearance_plane_point(2)]]
			}
		}
	}
	catch {	
		if {$clearance_plane < 0} {
			set clearance_plane [expr $clearance_plane*-1]
		}
	}

	set stepover_type ""
	catch {	set stepover_type $mom_stepover_type }
	if {$stepover_type == 1} {#Constant
		
		catch {	set stepover_type "Constant" }
		
		set unit_type 0
		catch {
			set unit_type $mom_stepover_distance_source
		}
		
		if {$unit_type == 4} {
			catch {
				set stepover "[format %.3f $mom_stepover_distance] %Tool"
			}
		} else {
			catch {
				set stepover "[format %.3f $mom_stepover_distance] mm"
			}
		}
	} elseif {$stepover_type == 2} {#Scallop
	
		catch {	set stepover_type "Scallop" }
		
		catch {	
			set stepover "[format %.3f $mom_stepover_scallop]"
			#set stepover $mom_stepover_scallop
		}
	} elseif {$stepover_type == 4} {#% Tool Flat
	
		catch {	set stepover_type "% Tool Flat" }
	
		catch {	
			set stepover "[format %.3f $mom_stepover_percent] %Flat"
		}
	} elseif {$stepover_type == 3} {#Multiple
	
		catch {	set stepover_type "Multiple" }
		
		catch {	
			set stepover "[format %.3f $mom_stepover_variable_values(0)]"
		}
	} else {
		catch {	set stepover_type "" }
	}

	
	global mom_common_depth_per_cut_type
	global mom_global_cut_depth_source
	global mom_global_cut_depth
	global mom_scallop_common_depth_per_cut

	global depth_of_cut
	#set depth_of_cut ""
	#set depth_per_cut_type -1
	catch {	
		set depth_per_cut_type $mom_common_depth_per_cut_type 
		
		if {$depth_per_cut_type == 0} {#Constant
			
			set unit_type 0
			catch {
				set unit_type $mom_global_cut_depth_source
			}
			
			if {$unit_type == 4} {
				catch {
					set depth_of_cut "[format %.3f $mom_global_cut_depth] %Tool"
				}
			} else {
				catch {
					set depth_of_cut "[format %.3f $mom_global_cut_depth] mm"
				}
			}
		} elseif {$depth_per_cut_type == 1} {#Scallop
			catch {	
				set depth_of_cut "$mom_scallop_common_depth_per_cut"
			}
		}	
	}


}

proc MOM_TOOL_HDR {} {
	#set output_log_file [open $::E_TABLE a]
	#puts $output_log_file "MOM_TOOL_HDR"
	#close $output_log_file
	
	global mom_tool_catalog_number
	global mom_tool_name
	global mom_toolpath_cutting_time
	global mom_toolpath_cutting_length
	global mom_tool_number
	global mom_tool_diameter
	global mom_tool_corner1_radius
	global mom_tool_length
	global mom_tool_tapered_shank_diameter
	global mom_tool_tapered_shank_length
	global mom_tool_tapered_shank_taper_length
	global mom_tool_flute_length
	global mom_tool_flutes_number
	global mom_tool_taper_angle
	global mom_tool_tip_angle
	global mom_holder_number_of_steps
	global mom_holder_step_sequence_number
	global mom_holder_step_diameter
	global mom_holder_step_length
	global mom_holder_step_taper_angle
	global mom_holder_step_corner_radius
	global mom_cutter_holder_libref
	global mom_libref
	global mom_tool_lower_corner_radius
	global mom_tool_upper_corner_radius
	global mom_tool_use_tapered_shank
	global mom_tool_type
	global mom_tool_point_angle
	global mom_tool_holder_offset

	global tool_name
	global cutting_time
	
	#201//03/06 Add mom_toolpath_cutting_length
	global cutting_length
	
	global tool_number
	global tool_dia
	global tool_radius
	global tool_length
	global shank_diameter
	global shank_length
	global shank_taper_length
	global flute_length
	global tool_length
	global flutes_number
	global taper_angle
	global tip_angle
	global holder_number_of_steps
	global holder_libref
	global tool_libref
	global TOOL_HLT_NO1
	global TOOL_HLT_NO2
	global tool_lower_corner_radius
	global tool_upper_corner_radius

	set shank_diameter ""
	set shank_length ""
	set shank_taper_length ""
	set flute_length ""
	set flutes_number ""
	set taper_angle 0
	set tip_angle 0
	set holder_number_of_steps 0
	set holder_libref ""
	set tool_libref ""
	set TOOL_HLT_NO1 ""
	set TOOL_HLT_NO2 ""
	set tool_lower_corner_radius ""
	set tool_upper_corner_radius ""
	set tool_use_tapered_shank ""
	#set cutting_length ""

	catch {	set tool_name $mom_tool_name }
	#catch {	set cutting_time $mom_toolpath_cutting_time }
	#catch {	set cutting_length $mom_toolpath_cutting_length }
	catch {	set tool_number $mom_tool_number }
	catch {	set tool_dia $mom_tool_diameter }
	catch {	set tool_radius $mom_tool_corner1_radius }
	catch {	set tool_length $mom_tool_length }
	catch {	set shank_diameter $mom_tool_tapered_shank_diameter }
	catch {	set shank_length $mom_tool_tapered_shank_length }
	catch {	set shank_taper_length $mom_tool_tapered_shank_taper_length }
	catch {	set flute_length $mom_tool_flute_length }
	catch {	set flutes_number $mom_tool_flutes_number }
	catch {	set taper_angle $mom_tool_taper_angle }
	catch {	set tip_angle $mom_tool_tip_angle }
	catch {	set holder_number_of_steps $mom_holder_number_of_steps }
	catch {	set tool_lower_corner_radius $mom_tool_lower_corner_radius }
	catch {	set tool_upper_corner_radius $mom_tool_upper_corner_radius }
	catch {	set tool_use_tapered_shank $mom_tool_use_tapered_shank }
	
	#2014/01/06 輸出刀把名稱
	catch {	set holder_libref $mom_cutter_holder_libref }

	#2014/01/15 輸出刀把1名稱
	#2014/01/15 輸出刀把2名稱
	set under_line_hd_1 [string first "_" $holder_libref]
	if {$under_line_hd_1 == -1} {
		set TOOL_HLT_NO1 $holder_libref
		set TOOL_HLT_NO2 ""
	} else {
		set start_index [expr $under_line_hd_1-1]
		catch {set TOOL_HLT_NO1 [string range $holder_libref 0 $start_index]}
		
		set start_index [expr $under_line_hd_1+1]
		catch {set TOOL_HLT_NO2 [string range $holder_libref $start_index end]}
	}
	
	
	#2014/01/15 輸出刀具名稱
	catch {	set tool_libref $mom_libref }
	
	#輸出刀把點資料
	global nx_holder
	global nx_holder_path
	set nx_holder ""
	set holder_str ""
	set nx_holder_path ""
	if { $holder_number_of_steps != 0 } {
		#global nc_name
		global HOLDER_DATA
		
		set nx_holder "$tool_name"
		append nx_holder "_holder_nx.dat"		
		set nx_holder_path "$HOLDER_DATA\\$nx_holder"
		set output_holder_file [open $nx_holder_path w]
		
		set nx_holder_path "SHOP_DOC/$nx_holder"
		
		set max_holder_dia 0
		set max_holder_num 0
		
		for {set i 0} {$i < $holder_number_of_steps} {incr i} {
			set holder_str ""
			append holder_str $mom_holder_step_sequence_number($i)
			append holder_str "\t"
			append holder_str [format %.4f $mom_holder_step_diameter($i)]
			append holder_str "\t"
			append holder_str [format %.4f $mom_holder_step_length($i)]
			append holder_str "\t"
			append holder_str [format %.4f $mom_holder_step_taper_angle($i)]
			append holder_str "\t"
			append holder_str [format %.4f $mom_holder_step_corner_radius($i)]

			if {$mom_holder_step_diameter($i) > $max_holder_dia} {
				set max_holder_dia $mom_holder_step_diameter($i)
				set max_holder_num $i+1
			}
			
			puts $output_holder_file $holder_str
		}

		close $output_holder_file
	}

	
	global upper_diameter
	global ncspeed_holder
	global ncspeed_holder_path
	
	set holder_str ""
	set holder_count 0
	set ncspeed_holder ""
	set ncspeed_holder_path ""
	
	if { $max_holder_num != 0 } {
		#global nc_name
		global HOLDER_DATA
		
		set ncspeed_holder "$tool_name"
		append ncspeed_holder "_holder_ncspeed.ncshld"		
		set ncspeed_holder_path "$HOLDER_DATA\\$ncspeed_holder"
		set output_holder_file [open $ncspeed_holder_path w]
		
		set ncspeed_holder_path "SHOP_DOC/$ncspeed_holder"
		
		set yPnt 0
		set chk 0
		
		for {set i 0} {$i < $max_holder_num} {incr i} {
			
			if {$i==0} {
				set xPnt [format %.4f [expr $mom_holder_step_diameter($i)/2]]
				set yPnt 0
				append holder_str " $xPnt $yPnt\n"
				set holder_count [expr $holder_count+1]
			}
			
			set computer_angle 0
			set upper_diameter($i) [expr $mom_holder_step_diameter($i)+0]
			catch {
				if {$mom_holder_step_taper_angle($i) > 0} {
					set computer_angle [expr 90-$mom_holder_step_taper_angle($i)]
				} elseif {$mom_holder_step_taper_angle($i) < 0} {
					set computer_angle [expr (90+$mom_holder_step_taper_angle($i))*-1]
				} else {
					set computer_angle 0
				}
				
				set upper_diameter($i) [expr $mom_holder_step_diameter($i)+($mom_holder_step_length($i)/tan($computer_angle*[PI]/180))*2]
			}
			
			if {$chk == 1} {
				set xPnt [format %.4f [expr $upper_diameter($i)/2]]
				append holder_str " $xPnt $yPnt\n"
				set holder_count [expr $holder_count+1]
			} elseif {$chk ==2} {
				set xPnt [format %.4f [expr $mom_holder_step_diameter($i)/2]]
				append holder_str " $xPnt $yPnt\n"
				set holder_count [expr $holder_count+1]
			}
			
			set index [expr $i+1]
			if {[expr $i+1] < $max_holder_num} {
			
				set bufPnt $yPnt
				set yPnt [format %.4f [expr $bufPnt+$mom_holder_step_length($i)]]
				set xPnt [format %.4f [expr $upper_diameter($i)/2]]
				
				if {[expr [format %.4f $upper_diameter($i)]] != [expr [format %.4f $mom_holder_step_diameter($index)]]} {
					if {$chk==1} {
						set chk 2
					} else {
						set chk 1
					}
				} else {
					set chk 0
				}
				append holder_str " $xPnt $yPnt\n"
				set holder_count [expr $holder_count+1]
			} else {
				set bufPnt $yPnt
				set yPnt [format %.4f [expr $bufPnt+$mom_holder_step_length($i)]]
				set xPnt [format %.4f [expr $upper_diameter($i)/2]]
				append holder_str " $xPnt $yPnt\n"
				set holder_count [expr $holder_count+1]
			}
		}
		
		set outstr ""
		append outstr "#3\n"
		append outstr "#1 $holder_count\n"
		append outstr $holder_str

		puts $output_holder_file $outstr
		close $output_holder_file
	}
	
	global mom_tool_length_adjust_register
	global oper_program
	global operation_type
	
	set H_code ""
	catch {	set H_code $mom_tool_length_adjust_register }
	
	#2014/02/17 Add
	if {$tool_use_tapered_shank == "No"} {
		set shank_diameter $tool_dia
		set shank_length 0
		set shank_taper_length 0
	}

	#catch {set tool_ext_len [expr $tool_length+$shank_length-$mom_tool_holder_offset]}	
	
	#2014/02/17 Add
	if {$mom_tool_type == "Drilling Tool"} {
		catch {	
			set tip_angle [expr $mom_tool_point_angle*180/3.1415926535897932384626433832795] 
		}
	}
	
	#mom_tool_catalog_number
	set tool_name ""
	
	catch {
		set tool_name $mom_tool_catalog_number
		#set tool_name $mom_tool_name
	}
	
	if {$tool_name == ""} {
		catch {
			set tool_name $mom_tool_name
			#set tool_name $mom_tool_catalog_number
		}
	}
	
	#2014/01/15 add tool_ext_len
	set under_line [string last "_" $tool_name]
	set start_index [expr $under_line+1]
	set tool_ext_len ""
	catch {set tool_ext_len [string range $tool_name $start_index end]}	
	

	global E_TABLE
	set output_log_file [open $E_TABLE a]

	puts $output_log_file "\{"
	puts $output_log_file "	\"data\":\["
	puts $output_log_file "		\{ \"name\":\"PROGRAM_ID\", \"value\":\"$oper_program\" \},"
	puts $output_log_file "		\{ \"name\":\"OPERATION_TYPE\", \"value\":\"$operation_type\" \},"
	
	#2014/01/15 modify
		
	catch {	
		puts $output_log_file "		\{ \"name\":\"TOOL_NAME\", \"value\":\"$tool_name\" \},"
	}
	#2014/01/15 delete
	#puts $output_log_file "		\{ \"name\":\"TOOL_NUMBER\", \"value\":\"$tool_number\" \},"
	#puts $output_log_file "		\{ \"name\":\"H_CODE\", \"value\":\"$H_code\" \},"
	
	
	puts $output_log_file "		\{ \"name\":\"TOOL_DIA\", \"value\":\"$tool_dia\" \},"
	puts $output_log_file "		\{ \"name\":\"TOOL_RADIUS\", \"value\":\"$tool_radius\" \},"
	
	#2014/01/15 add
	puts $output_log_file "		\{ \"name\":\"TOOL_EXT_LEN\", \"value\":\"$tool_ext_len\" \},"
	
	#2014/02/13 Add TOOL_LENGTH
	if {[expr $tool_dia] == [expr $shank_diameter]} {
		puts $output_log_file "		\{ \"name\":\"TOOL_LENGTH\", \"value\":\"$tool_ext_len\" \},"
	} elseif {[expr $tool_dia] < [expr $shank_diameter]} {
		puts $output_log_file "		\{ \"name\":\"TOOL_LENGTH\", \"value\":\"$tool_length\" \},"
	} else {
		puts $output_log_file "		\{ \"name\":\"TOOL_LENGTH\", \"value\":\"$tool_length\" \},"
	}
	
	#2014/02/13 Add
	puts $output_log_file "		\{ \"name\":\"TOOL_LOWER_CORNER_RADIUS\", \"value\":\"$tool_lower_corner_radius\" \},"
	puts $output_log_file "		\{ \"name\":\"TOOL_UPPER_CORNER_RADIUS\", \"value\":\"$tool_upper_corner_radius\" \},"
	
	puts $output_log_file "		\{ \"name\":\"CUTTING_TIME\", \"value\":\"$cutting_time\" \},"
	
	#2014/03/06 Add
	puts $output_log_file "		\{ \"name\":\"CUTTING_LENGTH\", \"value\":\"$cutting_length\" \},"
	
	puts $output_log_file "		\{ \"name\":\"TAPERED_SHANK_DIAMETER\", \"value\":\"$shank_diameter\" \},"
	puts $output_log_file "		\{ \"name\":\"TAPERED_SHANK_LENGTH\", \"value\":\"$shank_length\" \},"
	
	puts $output_log_file "		\{ \"name\":\"TAPERED_SHANK_TAPER_LENGTH\", \"value\":\"$shank_taper_length\" \},"
	puts $output_log_file "		\{ \"name\":\"TOOL_FULTE_LENGTH\", \"value\":\"$flute_length\" \},"
	puts $output_log_file "		\{ \"name\":\"TOOL_FLUTES_NO\", \"value\":\"$flutes_number\" \},"
	puts $output_log_file "		\{ \"name\":\"TOOL_TAPER_ANGLE\", \"value\":\"$taper_angle\" \},"
	puts $output_log_file "		\{ \"name\":\"TOOL_TIP_ANGLE\", \"value\":\"$tip_angle\" \},"
	
	puts $output_log_file "		\{ \"name\":\"HOLDER_POINT_NCSPEED\", \"value\":\"$ncspeed_holder_path\" \},"
	puts $output_log_file "		\{ \"name\":\"HOLDER_POINT\", \"value\":\"$nx_holder_path\" \},"

	#2014/01/06 輸出刀把名稱
	puts $output_log_file "		\{ \"name\":\"TOOL_HLT_NO_A\", \"value\":\"$holder_libref\" \},"

	#2014/01/15 輸出刀把名稱1
	puts $output_log_file "		\{ \"name\":\"TOOL_HLT_NO1\", \"value\":\"$TOOL_HLT_NO1\" \},"
	
	#2014/01/15 輸出刀把名稱2
	puts $output_log_file "		\{ \"name\":\"TOOL_HLT_NO2\", \"value\":\"$TOOL_HLT_NO2\" \}"
	
	puts $output_log_file "	\],"
	puts $output_log_file "	\"tool_path\":\["
	
	close $output_log_file

	
}

proc MOM_TOOL_FTR {} {
	#set output_log_file [open $::E_TABLE a]
	#puts $output_log_file "MOM_TOOL_FTR"
	#close $output_log_file
	global E_TABLE
	set output_log_file [open $E_TABLE a]
	puts $output_log_file "	\]"
	puts $output_log_file "\},"
	close $output_log_file
}

proc MOM_TOOL_BODY {} {
	global E_TABLE
	set output_log_file [open $E_TABLE a]
	#puts $output_log_file "MOM_TOOL_BODY"

	
	global temp_dir
	global env
	
	set temp_dir ""
	
	#set temp_dir $env(UGII_USER_DIR)
	set temp_dir $env(TEMP)
	
	#if {$temp_dir==""} {
    #    set temp_dir $env(TEMP)
    #}
	
    if {$temp_dir==""} {
        set temp_dir $env(TMP)
    }

    if {$temp_dir==""} {
        set temp_dir "d:\\"
    }
	
	global mom_operation_name
	global simple_tcl simple_def
	;#get cutting time, min_z and max_z by post processing
    set temp_file "$temp_dir\\shopdoc_office.txt"
    MOM_list_oper_path $mom_operation_name $simple_tcl $simple_def $temp_file
	
	#MOM_log_message "==MSG==oper_path $mom_operation_name $simple_tcl $simple_def $temp_file"

	
	global machine_time
	global cut_time
	global x_min
	global x_max
	global y_min
	global y_max
	global z_min
	global z_max
	global feed_to

	
    set fid [open $temp_file "r+"]
    gets $fid machine_time
	gets $fid cut_time
    gets $fid x_min
    gets $fid x_max
	gets $fid y_min
    gets $fid y_max
	gets $fid z_min
    gets $fid z_max
	gets $fid feed_to

    close $fid
    file delete -force $temp_file
	
	global work_name
	global nc_name
	global operation_name
	global cutting_time
	global tool_name
	global tool_number
	global feed
	global XY_Stock
	global operation_gif
	global RPM
	global depth_of_cut
	global operation_type
	global MAX_Z
	global clearance_plane
	#global H_code
	global tool_dia
	global tool_radius
	global tool_length
	global Z_Stock
	global stepover
	global stepover_type
	global cam_mill_type
	#global mom_tool_length_adjust_register
	global normal_dir

	
	
	#catch {	set H_code $mom_tool_length_adjust_register }
	catch {	set operation_name $mom_operation_name}	
	
	set operation_gif "SHOP_DOC/$operation_name.jpg"
	
	#set bufstr ""
	#append bufstr [format %.3f $x_min]
	#append bufstr "_"
	#append bufstr [format %.3f $y_min]
	#append bufstr "_"
	#append bufstr [format %.3f $z_min]
	#append bufstr "_"
	#append bufstr [format %.3f $feed_to]
	#set MAX_Z $bufstr
	
	set MAX_Z 0
	catch {
		if {$normal_dir == 0} {
			catch {	
				if {$feed_to < $y_min} {
					set MAX_Z [format %.3f $feed_to]
				} else {
					set MAX_Z [format %.3f $y_min]
				}
			}
		} elseif {$normal_dir == 1} {
			catch {	
				if {$feed_to < $x_min} {
					set MAX_Z [format %.3f $feed_to]
				} else {
					set MAX_Z [format %.3f $x_min]
				}
			}
		} elseif {$normal_dir == 2} {
			catch {	
				if {$feed_to < $z_min} {
					set MAX_Z [format %.3f $feed_to]
				} else {
					set MAX_Z [format %.3f $z_min]
				}
			}
		}
	}
	
	#puts $output_log_file "$work_name,$nc_name,$operation_name,$cutting_time,$tool_name,$tool_number,$feed,$XY_Stock,$operation_gif,$RPM,$depth_of_cut,$operation_type,$MAX_Z,$clearance_plane,$H_code,$tool_dia,$tool_radius,$tool_length,$Z_Stock,$stepover,$stepover_type"

	set out_str ""

	#append out_str "\{"
	
	#append out_str "work_name:'$work_name',"
	#append out_str "nc_name:'$nc_name',"
	#append out_str "operation_name:'$operation_name',"
	#append out_str "cutting_time:'$cutting_time',"
	#append out_str "tool_name:'$tool_name',"
	
	#append out_str "tool_number:'$tool_number',"
	#append out_str "feed:'$feed',"
	#append out_str "XY_Stock:'$XY_Stock',"
	
	
	
	#append out_str "operation_gif:'\<a href=\"#\" onMouseOver=\"showImg\(\\'$operation_gif\\'\);\" onMouseOut=\"hideImg\(\);\"\>',"
	#append out_str "RPM:'$RPM',"
	
	#append out_str "depth_of_cut:'$depth_of_cut',"
	#append out_str "operation_type:'$operation_type',"
	#append out_str "MAX_Z:'$MAX_Z',"
	#append out_str "clearance_plane:'$clearance_plane',"
	#append out_str "H_code:'$H_code',"
	
	#append out_str "tool_dia:'$tool_dia',"
	#append out_str "tool_radius:'$tool_radius',"
	#append out_str "tool_length:'$tool_length',"
	#append out_str "Z_Stock:'$Z_Stock',"
	#append out_str "stepover:'$stepover',"
	
	#append out_str "stepover_type:'$stepover_type',"
	#append out_str "\},"
	
	#global shank_diameter
	#global shank_length
	#global shank_taper_length
	#global flute_length
	#global flutes_number
	#global taper_angle
	#global tip_angle
	
	
	set find_index [string first "_" $operation_name]
	set start_index [expr $find_index-1]
	
	#2014/01/06
	set cam_mill_type ""
	catch {set cam_mill_type [string range $operation_name 0 $start_index]}
	
	puts $output_log_file "		\[";
	puts $output_log_file "			\{ \"name\":\"TOOL_PATH_GRAPH\", \"value\":\"$operation_name\" \},"
	puts $output_log_file "			\{ \"name\":\"TOOL_GRAPH_P\", \"value\":\"$operation_gif\" \},"
	
	#2014/01/06
	puts $output_log_file "			\{ \"name\":\"CAM_MILL_TYPE\", \"value\":\"$cam_mill_type\" \},"
	
	#puts $output_log_file "			\{ \"name\":\"cutting_time\", \"value\":\"$cutting_time\" \},"
	#puts $output_log_file "			\{ \"name\":\"tool_name\", \"value\":\"$tool_name\" \},"
	#puts $output_log_file "			\{ \"name\":\"tool_number\", \"value\":\"$tool_number\" \},"

	puts $output_log_file "			\{ \"name\":\"FEED_RATE\", \"value\":\"$feed\" \},"
	puts $output_log_file "			\{ \"name\":\"XY_STOCK\", \"value\":\"$XY_Stock\" \},"
	puts $output_log_file "			\{ \"name\":\"RPM\", \"value\":\"$RPM\" \},"
	puts $output_log_file "			\{ \"name\":\"DEPTH_PER_CUT\", \"value\":\"$depth_of_cut\" \},"
	#puts $output_log_file "			\{ \"name\":\"operation_type\", \"value\":\"$operation_type\" \},"
	
	
	#2014/01/15 Add
	puts $output_log_file "			\{ \"name\":\"MIN_X\", \"value\":\"$x_min\" \},"
	puts $output_log_file "			\{ \"name\":\"MAX_X\", \"value\":\"$x_max\" \},"
	puts $output_log_file "			\{ \"name\":\"MIN_Y\", \"value\":\"$y_min\" \},"
	puts $output_log_file "			\{ \"name\":\"MAX_Y\", \"value\":\"$y_max\" \},"
	puts $output_log_file "			\{ \"name\":\"MIN_Z\", \"value\":\"$z_min\" \},"
	
	
	
	puts $output_log_file "			\{ \"name\":\"MAX_Z\", \"value\":\"$MAX_Z\" \},"
	
	if {$clearance_plane == ""} {
		puts $output_log_file "			\{ \"name\":\"CLEARANE_PLANE\", \"value\":\"\[CLEARANE_PLANE\]\" \},"
	} else {
		puts $output_log_file "			\{ \"name\":\"CLEARANE_PLANE\", \"value\":\"$clearance_plane\" \},"
	}	
	
	#puts $output_log_file "			\{ \"name\":\"CLEARANE_PLANE\", \"value\":\"$clearance_plane\" \},"
	#puts $output_log_file "			\{ \"name\":\"H_CODE\", \"value\":\"$H_code\" \},"
	#puts $output_log_file "			\{ \"name\":\"tool_dia\", \"value\":\"$tool_dia\" \},"
	#puts $output_log_file "			\{ \"name\":\"tool_radius\", \"value\":\"$tool_radius\" \},"
	
	#puts $output_log_file "			\{ \"name\":\"tool_length\", \"value\":\"$tool_length\" \},"
	puts $output_log_file "			\{ \"name\":\"Z_STOCK\", \"value\":\"$Z_Stock\" \},"
	puts $output_log_file "			\{ \"name\":\"XY_STEPTYPE\", \"value\":\"$stepover\" \},"
	puts $output_log_file "			\{ \"name\":\"STEPOVER_TYPE\", \"value\":\"$stepover_type\" \}"
	
	#puts $output_log_file "			\{ \"name\":\"shank_diameter\", \"value\":\"$shank_diameter\" \},"
	#puts $output_log_file "			\{ \"name\":\"shank_length\", \"value\":\"$shank_length\" \},"
	#puts $output_log_file "			\{ \"name\":\"shank_taper_length\", \"value\":\"$shank_taper_length\" \},"
	#puts $output_log_file "			\{ \"name\":\"flute_length\", \"value\":\"$flute_length\" \},"
	#puts $output_log_file "			\{ \"name\":\"flutes_number\", \"value\":\"$flutes_number\" \},"
	#puts $output_log_file "			\{ \"name\":\"taper_angle\", \"value\":\"$taper_angle\" \},"
	#puts $output_log_file "			\{ \"name\":\"tip_angle\", \"value\":\"$tip_angle\" \},"
		
	puts $output_log_file "		\]"
	
	close $output_log_file


	#set nc_name ""
	set operation_name ""
	set cutting_time ""
	set cutting_length ""
	set tool_name ""
	set tool_number ""
	set feed ""
	set XY_Stock ""
	#set operation_gif ""
	set RPM ""
	set depth_of_cut ""
	set operation_type ""
	set MAX_Z ""
	set clearance_plane ""
	set H_code ""
	set tool_dia ""
	set tool_radius ""
	set tool_length ""
	set Z_Stock 0
	set stepover ""
	set stepover_type ""
	#set mom_stock_floor 0
	
}

#=============================================================================
proc MOM_MACHVIEW_HDR {} {
#=============================================================================
	#set output_log_file [open $::E_TABLE a]
	#puts $output_log_file "MOM_MACHVIEW_HDR"
	#close $output_log_file
}


#=============================================================================
proc MOM_MACHVIEW_FTR {} {
#=============================================================================
	#set output_log_file [open $::E_TABLE a]
	#puts $output_log_file "MOM_MACHVIEW_FTR"
	#close $output_log_file
}

;# added on 2007/4/18
proc hiset { v1 } {

	#set output_log_file [open $::E_TABLE a]
	#puts $output_log_file "hiset"
	#close $output_log_file
	
	return

         upvar $v1 v2
         if { [info exists v2] } { return 1 } else { return 0 }
}

;# If the top level program group is NONE,  return 1
;# when cycling object, it seems the mom_object_name for NONE program group did not be set in MOM_PROGRAM_BODY
;# So, the top level parent of objects which are under NONE program group is actually be set to NC_PROGRAM
proc skip_processing {} {

	#set output_log_file [open $::E_TABLE a]
	#puts $output_log_file "skip_processing"
	#close $output_log_file
	
	return

    global top_level_parent
    
    if {$top_level_parent == "NONE" || $top_level_parent == "NC_PROGRAM"} { return 1 } else { return 0 }
}

;# ouput text in data area
proc write_data_line { txt_line } {

	#set output_log_file [open $::E_TABLE a]
	#puts $output_log_file "write_data_line"
	#close $output_log_file
	
	return
	
	
    global current_row start_row default_row_count
    global combine_by_program_group
    global program_group_count
    global previous_row
    global colorindex_odd colorindex_even
	
    MOM_log_message "==MSG== write_data_line"
    
	set is_odd 0
    
    if {$current_row != $previous_row} {
        ;# check if need to add a new row
        if {[expr $current_row-$start_row+1] > $default_row_count} {
            set range [format "%d" $current_row]
            MOM_output_literal "$range,INSERT"
        }
        
        if {$combine_by_program_group} {
            set is_odd [expr $program_group_count % 2]
        } else {
            set is_odd [expr $current_row % 2]
        }
        if {$is_odd == 1} {
            MOM_output_literal "$colorindex_odd,ROWCOLOR,$current_row"
        } else {
            MOM_output_literal "$colorindex_even,ROWCOLOR,$current_row"
        }
        
        set previous_row $current_row
    }
    
    MOM_output_literal "$txt_line"
}

;# added on 2009/7/20
;# ouput text in data area, no clolor change
proc write_work_line { txt_line } {

	#set output_log_file [open $::E_TABLE a]
	#puts $output_log_file "write_work_line"
	#close $output_log_file
	
	return

    global current_row start_row default_row_count
    global previous_row
    
	 MOM_log_message "==MSG== write_work_line"
    
    if {$current_row != $previous_row} {
        ;# check if need to add a new row
        if {[expr $current_row-$start_row+1] > $default_row_count} {
            set range [format "%d" $current_row]
            MOM_output_literal "$range,INSERT"
        }
        
        set previous_row $current_row
    }
    
    MOM_output_literal "$txt_line"
}

proc PI {} {return 3.1415926535897932384626433832795}