
var nameCtrl;


function lyncImageMouseOut() {
	hideLyncPresencePopup();
}

function lyncImageClicked(img) {
	if (img != null) {
		var mail = $(img).data('email');
		showLyncPresencePopup(mail, img);
	}
}

function lyncImageLoaded(img) {
	if (img != null) {
		var mail = $(img).data('email');		
		showLyncStatus(mail, img);
	}
}


function InializeActiveX() {
	if (nameCtrl === undefined) {
		try {
			if (window.ActiveXObject) {
				nameCtrl = new ActiveXObject("Name.NameCtrl");
			} else {
				nameCtrl = CreateNPApiOnWindowsPlugin("application/x-sharepoint-uc");
			}
			attachLyncPresenceChangeEvent();
		} catch (ex) {
			alert("could not load ActiveXObject('Name.NameCtrl.1')");
		}

	}
}


function IsSupportedNPApiBrowserOnWin() {
	return true; // SharePoint does this: IsSupportedChromeOnWin() || IsSupportedFirefoxOnWin()
}

function IsNPAPIOnWinPluginInstalled(a) {
	return Boolean(navigator.mimeTypes) && navigator.mimeTypes[a] && navigator.mimeTypes[a].enabledPlugin;
}

function CreateNPApiOnWindowsPlugin(b) {
	var c = null;
	if (IsSupportedNPApiBrowserOnWin())
		try {
			c = document.getElementById(b);
			if (!Boolean(c) && IsNPAPIOnWinPluginInstalled(b)) {
				var a = document.createElement("object");
				a.id = b;
				a.type = b;
				a.width = "0";
				a.height = "0";
				a.style.setProperty("visibility", "hidden", "");
				document.body.appendChild(a);
				c = document.getElementById(b);
			}
		} catch (d) {
			c = null;
		}
	return c;
}

function attachLyncPresenceChangeEvent() {
	if (!nameCtrl) {
		return;
	}
	nameCtrl.OnStatusChange = onLyncPresenceStatusChange;
}

function onLyncPresenceStatusChange(userName, status, id) {
	var presenceClass = getLyncPresenceString(status);

	var userElementId = getId(userName);
	var userElement = $('#' + userElementId);
	removePresenceClasses(userElement);
	userElement.addClass(presenceClass);
}

function removePresenceClasses(jqueryObj) {
	jqueryObj.removeClass('available');
	jqueryObj.removeClass('offline');
	jqueryObj.removeClass('away');
	jqueryObj.removeClass('busy');
	jqueryObj.removeClass('donotdisturb');
	jqueryObj.removeClass('inacall');
}

function getId(userName) {
	return userName.replace('@', '_').replace('.', '_');
}

//В зависимости от статуса пользователя - выставляем картинку

function showLyncStatus(uri, img) {

	InializeActiveX();

	if (img != null) {

		var status = getLyncPresenceString(nameCtrl.GetStatus(uri, "lyncspan"));
		switch (status) {
			case "donotdisturb":
				if ($(img).attr("src") != "/Images/LyncStatuses/donotdisturb.png")
					$(img).attr("src", "/Images/LyncStatuses/donotdisturb.png");
				break;
			case "offline":
				if ($(img).attr("src") != "/Images/LyncStatuses/offline.png")
					$(img).attr("src", "/Images/LyncStatuses/offline.png");
				break;
			case "available":
				if ($(img).attr("src") != "/Images/LyncStatuses/online.png")
					$(img).attr("src", "/Images/LyncStatuses/online.png");
				break;
			case "away":
				if ($(img).attr("src") != "/Images/LyncStatuses/away.png")
					$(img).attr("src", "/Images/LyncStatuses/away.png");
				break;
			case "busy":
				if ($(img).attr("src") != "/Images/LyncStatuses/busy.png")
					$(img).attr("src", "/Images/LyncStatuses/busy.png");
				break;
			default:
		}
	}
}


//получение статуса пользователя

function getLyncPresenceString(status) {

	switch (status) {
		case 0:
			return 'available';
		case 1:
			return 'offline';
		case 2:
		case 4:
		case 16:
			return 'away';
		case 3:
		case 5:
			return 'inacall';
		case 6:
		case 7:
		case 8:
		case 10:
			return 'busy';
		case 9:
		case 15:
			return 'donotdisturb';
		default:
			return '';
	}
}

function showLyncPresencePopup(userName, target) {
	if (!nameCtrl) {
		return;
	}

	var eLeft = $(target).offset().left;
	var x = eLeft - $(window).scrollLeft();

	var eTop = $(target).offset().top;
	var y = eTop - $(window).scrollTop();

	nameCtrl.ShowOOUI(userName, 0, x, y);
}

function hideLyncPresencePopup() {
	if (!nameCtrl) {
		return;
	}
	nameCtrl.HideOOUI();
}