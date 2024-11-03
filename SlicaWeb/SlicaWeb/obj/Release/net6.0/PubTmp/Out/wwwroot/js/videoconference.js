// Create Agora client
AgoraRTC.setLogLevel(2);
var client = AgoraRTC.createClient({
    mode: "live",
    codec: "vp8"
});

//AgoraRTC.setLogLevel(2);

//The output log level.

//0: DEBUG.Output all API logs.
//1: INFO.Output logs of the INFO, WARNING and ERROR level.
//2: WARNING.Output logs of the WARNING and ERROR level.
//3: ERROR.Output logs of the ERROR level.
//4: NONE.Do not output any log.
// Create Agora RTM client
//const clientRTM = AgoraRTM.createInstance($("#appid").val(), {
//    enableLogUpload: false
//});

// RTM Global Vars
var isLoggedIn = false;

var localTracks = {
    videoTrack: null,
    audioTrack: null
};
var remoteUsers = {};
// Agora client options
var options = {
    appid: null,
    channel: null,
    uid: null,
    token: null,
    accountName: null,
    role: "host",
    rtmToken: null,
    rtmUid: null
};

$("#rtmjoin").click(async function (e) {
    
    e.preventDefault();
    $("#rtmjoin").attr("disabled", true);
    try {
        options.appid = $("#appid").val();
        options.token = $("#token").val();
        options.channel = $("#channel").val();
        options.accountName = $('#uid').val();
        options.rtmToken = $('#rtmtoken').val();
        options.rtmUid = $('#hdnUserID').val();
        $("#join").click();
    } catch (error) {
        console.error(error);
    } finally {
        $("#mic-btn").attr("disabled", false);
        $("#video-btn").attr("disabled", false);
        $("#leave").attr("disabled", false);
        $("#rtmjoin").hide();
    }

    // clTmr = setInterval(callTimer, 1000);
})

$("#join").click(async function (e) {
    e.preventDefault();
    $("#join").attr("disabled", true);
    try {
        options.appid = $("#appid").val();
        options.token = $("#token").val();
        options.channel = $("#channel").val();
        options.accountName = $('#uid').val();
        options.rtmToken = $('#rtmtoken').val();
        options.rtmUid = $('#hdnUserID').val();
        var interviewDateTime = $("#interviewDateTime").val();
        var durationInMin = $("#durationInMin").val();
        var minutes = $("#durationInMin").val();
        //if (interviewDateTime != null || interviewDateTime != "") {
        //    minutes = (durationInMin - (Math.round(new Date() - new Date(interviewDateTime))) / (3600 * 60));
        //}
        countdownMinus("countdown-timer", minutes, 0);
        await join();


    } catch (error) {
        console.error(error);
    } finally {
        $("#mic-btn").attr("disabled", false);
        $("#video-btn").attr("disabled", false);
        $("#leave").attr("disabled", false);
        $("#join").hide();
    }

    // clTmr = setInterval(callTimer, 1000);
})

$("#leave").click(async function (e) {
    if (confirm('Do you want to leave meeting?')) {
        closeMeeting();
    }
});


async function join() {
    //$("#mic-btn").prop("disabled", false);
    //$("#video-btn").prop("disabled", false);
    //$("#mic-btn").removeClass("d-none");
    //$("#video-btn").removeClass("d-none");
    $("#waitingbtn").hide();
    // add event listener to play remote tracks when remote user publishs.
    client.on("user-published", handleUserPublished);
    client.on("user-left", handleUserLeft);
    client.on("publish", handlePublish);
    client.on("unpublish", handleunpublish);
    client.on("subscribe", handlesubscribe);
    client.on("unsubscribe", handleunsubscribe);
    client.on("user-info-updated", handleuserinfoupdated);
    // join a channel and create local tracks, we can use Promise.all to run them concurrently
    [options.uid, localTracks.audioTrack, localTracks.videoTrack] = await Promise.all([
        // join the channel
        client.join(options.appid, options.channel, options.token || null, options.rtmUid),
        // create local tracks, using microphone and camera
        AgoraRTC.createMicrophoneAudioTrack(),
        AgoraRTC.createCameraVideoTrack()
    ]);

    // Set the role of the local user to "audience"
    await client.setClientRole(options.role);
    //await saveAgoraUser(options.uid);
    //await clientRTM.logout();
    //await RTMJoin();
    // play local video track
    localTracks.videoTrack.play("local-player");
    $("#local-player-name").text(`localVideo(${options.uid})`);
    // publish local tracks to channel
    await client.publish(Object.values(localTracks));
    console.log("publish success");
}
async function leave() {
    for (trackName in localTracks) {
        var track = localTracks[trackName];
        if (track) {
            track.stop();
            track.close();
            $('#mic-btn').prop('disabled', true);
            $('#video-btn').prop('disabled', true);
            localTracks[trackName] = undefined;
        }
    }
    // remove remote users and player views
    remoteUsers = {};
    $("#remote-playerlist").html("");
    // leave the channel
    await client.leave();
    $("#local-player-name").text("");
    $("#join").attr("disabled", false);
    $("#leave").attr("disabled", true);
    console.log("client leaves channel success");
    window.location.href = "/dashboard";
}
async function subscribe(user, mediaType) {
    const uid = user.uid;
    // subscribe to a remote user
    await client.subscribe(user, mediaType);
    console.log("subscribe success");
    if (mediaType === 'video') {
        if ($(`#player-wrapper-${uid}`).html() == undefined) {
            $("#remote-playerlist").append($(`
      <li id="player-wrapper-${uid}" class="bg-dark">
        <p class="player-name-${uid}" style="background: #e3d1b0;padding: 3px 10px;color: #567384;font-weight: 600;">${$(`#anchor-${uid}`).html()}</p>
        <div id="player-${uid}" class="player">
        </div>
      </li>
    `));
        } else {
            $("#remote-playerlist").find(`#player-wrapper-${uid}`).html(
                `
        <p class="player-name-${uid}" style="background: #e3d1b0;padding: 3px 10px;color: #567384;font-weight: 600;">${$(`.player-name-${uid}`).html()}</p>
        <div id="player-${uid}" class="player">
        </div>
    `
            );
        }
        user.videoTrack.play(`player-${uid}`);
    }
    if (mediaType === 'audio') {
        user.audioTrack.play();
    }
}


function handleuserinfoupdated(uid, msg) {
    //getAgoraUser(options.uid, msg);
}
function handlePublish(tracks) {
}
function handleunpublish(tracks) {
}
function handlesubscribe(user, mediaType) {
}
function handleunsubscribe(user, mediaType) {
}



// Handle user publish
function handleUserPublished(user, mediaType) {
    const id = user.uid;
    remoteUsers[id] = user;
    subscribe(user, mediaType);
}

// Handle user left
function handleUserLeft(user) {
    const id = user.uid;
    delete remoteUsers[id];
    $(`#player-wrapper-${id}`).remove();
    leaveWhenAdmin(id);
}

// Initialise UI controls
enableUiControls();

// Action buttons
function enableUiControls() {
    $("#mic-btn").click(function () {
        toggleMic();
        var json = {
            uid: options.uid,
            text: '',
            type: `audio${$("#mic-icon").hasClass('fa-microphone-slash') ? 'Mute' : 'Unmute'}`
        };
        sendMessage(JSON.stringify(json), $("#host").html());
    });
    $("#video-btn").click(function () {
        toggleVideo();
        var json = {
            uid: options.uid,
            text: '',
            type: `video${$("#video-icon").hasClass('fa-video-slash') ? 'Mute' : 'Unmute'}`
        };
        sendMessage(JSON.stringify(json), $("#host").html());
    });
}

// Toggle Mic
function toggleMic() {
    if ($("#mic-icon").hasClass('fa-microphone')) {
        localTracks.audioTrack.setEnabled(false);
        console.log("Audio Muted.");
    } else {
        localTracks.audioTrack.setEnabled(true);
        console.log("Audio Unmuted.");
    }
    $("#mic-icon").toggleClass('fa-microphone').toggleClass('fa-microphone-slash');
}

// Toggle Video
function toggleVideo() {
    if ($("#video-icon").hasClass('fa-video')) {
        localTracks.videoTrack.setEnabled(false);
        console.log("Video Muted.");
    } else {
        localTracks.videoTrack.setEnabled(true);
        console.log("Video Unmuted.");
    }
    $("#video-icon").toggleClass('fa-video').toggleClass('fa-video-slash');
}

// Toggle Speaker
function toggleSpeaker(isOn) {
    if (isOn) {
        for (var i = 0; i < client.remoteUsers.length; i++) {
            client.remoteUsers[i].audioTrack.setVolume(100);
        }
        $("#speaker-icon").parent().parent().hide();
    } else {
        for (var i = 0; i < client.remoteUsers.length; i++) {
            client.remoteUsers[i].audioTrack.setVolume(0);
        }
        $("#speaker-icon").parent().parent().show();
    }
}

//video timer

function tmrAlert() {
    $("#lblcounter").text("just 5 min remaining");
}

async function closeMeeting() {
    
    //var interviewGrpId = $("#InterviewGroupid").val();
    //var _data = {
    //    InterviewGroupId: interviewGrpId,
    //    Status: 2
    //}
    //$.ajax({
    //    type: 'POST',
    //    url: baseUrl + '/Home/ChangeMeetingStatus',
    //    data: JSON.stringify(_data),
    //    contentType: 'application/json',
    //    processData: false,
    //    cache: false,
    //    success: async function (r) {
    //            await leave();
    //    },
    //    error: async function (err) {
    //        await leave();
    //    }
    //});   
   await window.dotNetHelper.invokeMethodAsync('EndMeeting', false);

                await leave();
            
    
}
function countdownMinus(elementName, minutes, seconds) {

    var element, endTime, hours, mins, msLeft, time;
    function twoDigits(n) { return (n <= 9 ? "0" + n : n); }

    element = document.getElementById(elementName);
    endTime = (+new Date) + 1000 * (60 * minutes + seconds) + 500;
    updateTimer();

}
    function updateTimer() {
        msLeft = endTime - (+new Date);

        if (msLeft < 1000) {
            //element.innerHTML = "Time is up!";
            //$("#btncompleteAppointment").click();
            //$("#completeAppointmentClosebtn").hide();
            //$("#lblcounter").text("Your meeting time is over.");
            closeMeeting();
        }
        //else if (msLeft < 300000) {
        //    /*setinterval(tmralert, 60000);*/
        //    tmrAlert();
        //    setTimeout(updateTimer, time.getUTCMilliseconds() + 500);
        //}
        else {
            time = new Date(msLeft);
            hours = time.getUTCHours();
            mins = time.getUTCMinutes();
            /*element.innerHTML = (hours ? hours + ':' + twoDigits(mins) : mins) + ':' + twoDigits(time.getUTCSeconds());*/
            hours = "00";
            element.innerHTML = (hours ? hours + ':' + twoDigits(mins) : mins) + ':' + twoDigits(time.getUTCSeconds());
            setTimeout(updateTimer, time.getUTCMilliseconds() + 500);
        }
    }

function SetDotNetHelper(dotNetHelper) {
    window.dotNetHelper = dotNetHelper;
}
// Start time func.
var startTimerSec = 0;
function pad(val) { return val > 9 ? val : "0" + val; }
function startTimer(elemId) {
    setInterval(function () {
        $seconds = pad(++startTimerSec % 60);
        $minutes = pad(parseInt(startTimerSec / 60, 10) % 60);
        $hours = pad(parseInt(startTimerSec / 3600, 10));
        $(`#${elemId}`).html(`${$hours}:${$minutes}:${$seconds}`);
    }, 1000);
}

