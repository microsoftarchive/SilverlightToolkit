//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------
if (typeof (Sys) !== "object") {
    Sys = {};
}
if (typeof (Sys.Navigator) !== "object") {
    /// <summary>
    /// Default constructor.
    /// </summary>
    Sys._Navigator = function Sys$_Navigator() {
        this.useFrame = this._updating = this._dc = false;
        this._poll = null;
        this._hash = "";
        this._listeners = [];
        this._initialize();
    }
    /// <summary>
    /// Navigates the browser location fragment (hash).
    /// </summary>
    function Sys$_Navigator$navigate(h, t) {
        h = this._cleanHash(h);
        if (t == null) {
            t = document.title;
        }
        this._updating = true;
        this._hash = h;
        if (!this._suppressEvent && this.useFrame) {
            this._navIFrame(h, t);
        }
        this._updateUI(h, t);
        this.navigated();
        this._updating = false;
    }
    /// <summary>
    /// This method is called to raise the navigated event.
    /// </summary>
    function Sys$_Navigator$navigated() {
        for (var i = 0; i < this._listeners.length; i++) {
            var func = this._listeners[i];
            if (typeof (func) === "function") {
                func(document.location.toString());
            }
        }
    }
    /// <summary>
    /// Registers a Silverlight object for navigation callbacks.
    /// </summary>
    function Sys$_Navigator$register(plugin, obj, method) {
        this._listeners.push(function(url) {
            var tries = 0;
            while (++tries < 3) {
                try {
                    plugin.content[obj][method](url); break;
                } catch (e) { }
            }
        });
    }
    /// <summary>
    /// Checks for deep links.
    /// </summary>
    function Sys$_Navigator$_checkDeepLink() {
        with (Sys.Navigator) {
            if (!_updating) {
                var h = _getHash();
                if (_hash !== h) {
                    _dc = true; navigate(h, null); _dc = false;
                }
            }
        }
    }
    /// <summary>
    /// Cleans a hash value.
    /// </summary>
    function Sys$_Navigator$_cleanHash(h) {
        h = h || "";
        return (h.charAt(0) == "#") ? h.substring(1) : h;
    }
    /// <summary>
    /// This method attempts to detect browser versions.
    /// </summary>
    function Sys$_Navigator$_detectBrowser() {
        var v, nav = navigator.userAgent.toUpperCase();
        if (null !== (v = nav.match(/(?:MSIE\s)([.0-9]+)/))) {
            if (document.documentMode) {
                this.IE8 = true;
            }
            if (!this.IE8 || document.documentMode < 8) {
                this.useFrame = true; this._makeFrame();
            }
        }
        else if (null !== (v = nav.match(/FIREFOX/))) {
            this.isFF = true;
        }
    }
    /// <summary>
    /// This method returns the current document.location.hash value.
    /// </summary>
    function Sys$_Navigator$_getHash() {
        var i = document.location.href.indexOf('#');
        return i > 0 ? this._cleanHash(document.location.href.substring(i + 1)) : "";
    }
    /// <summary>
    /// This method initializes the singleton instance.
    /// </summary>
    function Sys$_Navigator$_initialize() {
        this._detectBrowser();
        this._poll = setInterval(this._checkDeepLink, 75);
    }
    /// <summary>
    /// This method creates a hidden <FRAME> for IE7 and below.
    /// </summary>
    function Sys$_Navigator$_makeFrame() {
        var f = this._iframe = document.createElement("iframe");
        f.style.display = "none";
        document.body.appendChild(f);
    }
    /// <summary>
    /// This method navigates a hidden <IFRAME> for IE7 and below.
    /// </summary>
    function Sys$_Navigator$_navIFrame(h, t) {
        this._suppressEvent = true;
        var d = this._iframe.contentWindow.document;
        h = ec(h);
        t = ec(t);
        d.open("javascript:'<html/>'");
        d.write("<html><head><title>" + t + "</title><scr" + "ipt>parent.Sys.Navigator._onIFrame('" +
                  h + "','" + t + "')</scr" + "ipt></head><body/></html>");
        d.close();
        function ec(s) {
            return encodeURIComponent(s).replace(/\'/g, "\\\'");
        }
    }
    /// <summary>
    /// This method is called when a child IFrame has loaded
    /// </summary>
    function Sys$_Navigator$_onIFrame(h, t) {
        if (!this._suppressEvent) {
            this._updating = true;
            this._hash = h = this._cleanHash(decodeURIComponent(h));
            this._updateUI(h, decodeURIComponent(t));
            this.navigated();
        }
        this._suppressEvent = this._updating = false;
    }
    /// <summary>
    /// This method is called to update the browser hash and title.
    /// </summary>
    function Sys$_Navigator$_updateUI(h, t) {
        if (!this._dc) {
            if ((!h || h == "") && this.isFF) {
                document.location = "#";
            }
            else {
                document.location.hash = this._hash = h;
            }
        }
        document.title = t;
    }
    // Define Navigator prototype members
    Sys._Navigator.prototype =
    {
        navigate: Sys$_Navigator$navigate,
        navigated: Sys$_Navigator$navigated,
        register: Sys$_Navigator$register,
        _checkDeepLink: Sys$_Navigator$_checkDeepLink,
        _cleanHash: Sys$_Navigator$_cleanHash,
        _detectBrowser: Sys$_Navigator$_detectBrowser,
        _getHash: Sys$_Navigator$_getHash,
        _initialize: Sys$_Navigator$_initialize,
        _makeFrame: Sys$_Navigator$_makeFrame,
        _navIFrame: Sys$_Navigator$_navIFrame,
        _onIFrame: Sys$_Navigator$_onIFrame,
        _updateUI: Sys$_Navigator$_updateUI
    };
    // Spin up new instance
    Sys.Navigator = new Sys._Navigator();
}
