var app = require(process.env.APPDATA + '\\npm\\node_modules\\express')();
var http = require('http').Server(app);
var io = require(process.env.APPDATA + '\\npm\\node_modules\\socket.io')(http);

var port = 9104;
var isValidPort = true;

var help = function (){
	isValidPort = false;
	console.log('Usage : node socket.io.server.js [Port Number]');
};

var command = function (repeat, notices, commands, callbacks) {
	if (repeat && Array.isArray(notices) && Array.isArray(commands) && Array.isArray(callbacks)) {
		if (notices.length <= 0) {
			throw 'command: No notice.';
		} else if (commands.length <= 0) {
			throw 'command: No command.';
		} else if (callbacks.length <= 0) {
			throw 'command: No callback.';
		} else if (commands.length != callbacks.length) {
			throw 'command: Command count not match to callback count.';
		}
		
		var i = 0;
		for (i = 0; i < notices.length; i++)
			console.log(notices[i]);
		
		process.stdin.resume();
		process.stdin.once('data', function (answer) {
			if (answer) {
				answer = answer.toString().trim();
				
				for (i = 0; i < commands.length; i++) {
					if (answer.toLowerCase() == commands[i].toLowerCase()) {
						callbacks[i]();
					}
				}
			}
			
			if (repeat) {
				command(repeat, notices, commands, callbacks);
			}
		});
	}
};

if (process.argv.length >= 2 && process.argv.length <= 3) {	
	if (process.argv.length == 3) {
		if (!isNaN(process.argv[2])) {
			port = parseInt(process.argv[2]);
		} else {
			help();
		}
	}
	
	if (isValidPort) {
		app.get('/', function (req, res) {
			res.send('<h1>Hello world</h1>');
		});

		http.listen(port, function () {
			console.log('listening on *:' + port);
			
			var notices = [ 'Input \"Exit\" to stop server. ' ];
			var commands = [ 'exit' ];
			var callbacks = [ function() { process.exit(1); } ];
			
			command(true, notices, commands, callbacks);
		});

		var connection = 'connection';
		var disconnect = 'disconnect';
		var error = 'error';

		var login = 'login';
		var logout = 'logout';

		var loginmaster = 'loginmaster';
		var logoutmaster = 'logoutmaster';

		var members = 'members';
		var screen = 'screen';
		var screenrequest = 'screenrequest';

		var mouseclick = 'mouseclick';

		var nicknameList = {};
		var idList = {};
		var memberList = [];

		var masterNickname = 'master';
		var screenRequestList = {};

		var onLogin = function (socket, loginedNickname) {
		    if (loginedNickname) {
		        if (!idList[loginedNickname]) {
		            console.log(login + ' : ' + loginedNickname);

		            nicknameList[socket.id] = loginedNickname;
		            idList[loginedNickname] = socket.id;

		            memberList.push({
		                id: socket.id,
		                nickname: loginedNickname
		            });
		            emit(masterNickname, members, memberList);

		            return true;
		        }
		    }

		    return false;
		};

		var emitLogout = function (socket, message) {
		    if (socket && typeof socket.emit == 'function') {
		        socket.emit(logout, message);
		        socket.disconnect();
		    }
		};

		var getSocket = function (memberNickname) {
		    if (memberNickname && idList[memberNickname]) {
		        return io.sockets.connected[idList[memberNickname]];
		    } else {
		        return null;
		    }
		};

		var emit = function (memberNickname, event, data) {
		    var memberSocket = getSocket(memberNickname);

		    if (memberSocket) {
		        if (data) {
		            memberSocket.emit(event, data);
		        } else {
		            memberSocket.emit(event);
		        }
		    }
		};

		io.on(connection, function (socket) {
		    socket.on(loginmaster, function (data) {
		        if (!onLogin(socket, masterNickname)) {
		            emitLogout(socket, '중복된 닉네임입니다.');
		        } else {
		            socket.broadcast.emit(loginmaster);
		        }
		    });

		    socket.on(login, function (data) {
		        if (!data || !data.nickname || data.nickname.trim().length == 0 || data.nickname.toLowerCase() == masterNickname || !onLogin(socket, data.nickname)) {
		            emitLogout(socket, '중복된 닉네임입니다.');
		        }
		    });

		    socket.on(screen, function (data) {
		        if (data && data.nickname) {
		            if (screenRequestList[socket.id]) {
		                screenRequestList[socket.id] = false;

		                if (data.nickname == masterNickname) {
		                    socket.broadcast.emit(screen, data);
		                } else if (idList[masterNickname] && io.sockets.connected[idList[masterNickname]]) {
		                    io.sockets.connected[idList[masterNickname]].emit(screen, data);
		                }
		            }
		        }
		    });

		    socket.on(screenrequest, function (data) {
		        if (data && data.nickname) {
		            if (!data.selectedMember) {
		                data.selectedMember = masterNickname;
		            }

		            if (data.nickname != data.selectedMember && idList[data.selectedMember] && io.sockets.connected[idList[data.selectedMember]] && !screenRequestList[idList[data.selectedMember]]) {
		                screenRequestList[idList[data.selectedMember]] = true;
		                io.sockets.connected[idList[data.selectedMember]].emit(screenrequest);
		            }
		        }
		    });

		    socket.on(mouseclick, function (data) {
		        if (!data.selectedMember) {
		            data.selectedMember = masterNickname;
		        }

		        if (data.selectedMember != nicknameList[socket.id]) {
		            emit(data.selectedMember, mouseclick, data);
		        }
		    });

		    socket.on(disconnect, function () {
				var i = 0;
				for (i = 0; i < memberList.length; i++) {
					if (memberList[i].id == socket.id && memberList[i].nickname == nicknameList[socket.id]) {
						break;
					}
				}

				var logoutedNickname = null;
				if (i < memberList.length) {
				    logoutedNickname = nicknameList[socket.id];
					console.log(logout + ' : ' + logoutedNickname);

					memberList.splice(i, 1);
					delete idList[logoutedNickname];
					delete nicknameList[socket.id];
				}

				if (logoutedNickname) {
				    if (logoutedNickname == masterNickname) {
				        socket.broadcast.emit(logoutmaster);
				    }
				}

				emit(masterNickname, members, memberList);
			});

			socket.on(error, function (data) {
			    console.log(data);
			});
		});
	}
} else {
	help();
}