/**
 * Author: LongH
 * Description: Cung cấp các API gửi request
 * Date: 10/07/2011
 * FWVersion: 3.3
 **/
sd = {};
sd.connector = {
    /**
     * Author: LongH
     * Description: Bổ sung tham số sync, cho phép gửi request ajax đồng bộ
     * Date: 16/06/2011
     * FWVersion: 3.3
     **/
    submittingRequest : [], // Cac request co chua token
    setOnFocus: function (div) {
        var inputs = document.getElementById(div).getElementsByTagName("input");
        var isFirstText = true;
        if (inputs != null && inputs.length > 0) {
            var i = 0;
            for (i = 0; i < inputs.length; i++) {
                if (inputs[i].getAttribute("type") == "text") {
                    if (checkIsVisible(inputs[i])) {
                        if (isFirstText) {
                            isFirstText = false;
                            inputs[i].focus();
                            return;
                        }
                    }
                    //inputs[i].setAttribute("onfocus", "onItemFocus(this);");
                }
            }
        }
    },
    
    postComplete: function (url) {
        var i;
        for (i = 0; i < sd.operator.urlStack.length; i++) {
            if (sd.operator.urlStack[i] == url) {
                sd.operator.urlStack.splice(i, 1);
            }
            if (sd.operator.urlStack.length == 0) {
                sd.operator.displayWaitScreen(false);
            }
        }
        for (i = 0; i < sd.connector.submittingRequest.length; i++) {
            if (sd.connector.submittingRequest[i] == url) {
                sd.connector.submittingRequest.splice(i, 1);
            }
        }

    },
    post: function ( /*String url, String area, String formId, Object param, Function callback, Function errback, String responseType, Boolean sync*/ ) {
        var _url, _area, _param, _form, _callback, _errback, _responseType, _sync;
        var args = arguments;

        // [ Collect arguments
        _url = args[0];
        sd.operator.urlStack.push(_url);
        if(_url.indexOf("struts.token.name=token")>=0){
            sd.connector.submittingRequest.push(_url);
            //console.log(sd.connector.submittingRequest);
        }

        _param = null;
        _form = null;
        _responseType = "text";

        if (args.length >= 2) {
            _area = args[1];
            if (args.length >= 3) {
                _form = dojo.byId(args[2]);
                if (args.length >= 4) {
                    _param = args[3];
                }
                if (args.length >= 5) {
                    _callback = args[4];
                    if (args.length >= 6) {
                        _errback = args[5];
                        if (args.length >= 7) {
                            _responseType = args[6];
                            if (args.length >= 8) {
                                _sync = args[7];
                            }
                        }
                    }
                }
            }
        }

        //]Datbt

        var parameter = {
            url: _url,
            preventCache: true,
            load: function (response, ioArgs) {
                try {
                    if (_responseType == "text") {
                        var node = null;

                        if (_area) {
                            node = dojo.byId(_area);
                        }

                        if (node != null && node != undefined) {
                            try {
                                sd.operator.freeWidgets(node);
                                sd.operator.allowedToExecJS = false;
                                node.innerHTML = response;
                                dojo.parser.parse(node);
                            } catch (e) {
                                console.log("Error in post.load, \nsd.operator.freeWidgets, dojo.parser.parse:\n" + e.message);
                            }
                        } else {
                            console.log("sd.connector.post: The area with areaId = '" + _area + "' is null");
                        }

                        try {
                            var mixed = sd.operator.parse(response);
                            sd.operator.allowedToExecJS = true;
                            sd.operator.execScript(mixed.scripts);
                        } catch (e) {
                            console.log(e);
                        }
                    }
                } catch (e) {
                    console.log("Error in post.load:\n" + e.message);
                }

                if (_callback) {
                    try {
                        _callback.call(this, response, ioArgs);
                    } catch (e) {
                        console.log(e.message);
                    }
                }

                //sd.operator.displayWaitScreen(false);
                sd.connector.postComplete(_url);
                if (_area != null && _area != "") {
                    sd.connector.setOnFocus(_area);
                }
            },
            error: function (error, ioArgs) {
                //sd.operator.displayWaitScreen(false);
                //sd.operator.displayErrorWhileSendingRequest(error);
                //console.log( "Inside post.error:\n" + sd.util.dump(error, 1) );

                if (_errback) {
                    try {
                        _errback.call(this, error, ioArgs);
                    } catch (e) {
                        console.log("Error in your onFail:\n" + e.message);
                    }
                }
                sd.connector.postComplete(_url);
            },
            handleAs: _responseType
        };

        if (_param) {
            parameter.content = _param;
        }
        if (_form) {
            parameter.form = _form;
        }

        if (_sync === true || _sync === false) {
            parameter.sync = _sync;
        }

        if (sd.operator.timeout) {
            parameter.timeout = sd.operator.timeout;
        }

        sd.operator.displayWaitScreen(true);

        dojo.xhrPost(parameter);
    },
    updateArea: function (/*{String url, String areaId, String formId, Object param, Function onSuccess, Function onFail, Boolean sync}*/ inputParam) {
        var url, areaId, formId, param, callback, errback, sync,
                responseType = "text";

        url = inputParam.url;
        areaId = inputParam.areaId;
        formId = inputParam.formId;
        param = inputParam.param;
        callback = inputParam.onSuccess;
        errback = inputParam.onFail;
        sync = inputParam.sync;

        sd.connector.post(url, areaId, formId, param, callback, errback, responseType, sync);
    },
    getJSON: function (/*{String url, String formId, Object param, Function onSuccess, Function onFail}*/ inputParam) {
        var url, formId, param, callback, errback, sync,
                responseType = "json";

        url = inputParam.url;
        formId = inputParam.formId;
        param = inputParam.param;
        callback = inputParam.onSuccess;
        errback = inputParam.onFail;
        sync = inputParam.sync;

        sd.connector.post(url, null, formId, param, callback, errback, responseType, sync);
    },
    get: function () {
        var _url, _param, _callback;
        var args = arguments;

        // [ Collect arguments
        _url = args[0];
        _param = args[1];
        _callback = args[2];

        var request;
        if (window.XMLHttpRequest) {// code for IE7+, Firefox, Chrome, Opera, Safari
            request = new XMLHttpRequest();
        } else {// code for IE6, IE5
            request = new ActiveXObject("Microsoft.XMLHTTP");
        }
        request.open("POST", _url, false);
        request.setRequestHeader("Content-type", "application/json; charset=UTF-8");
        //console.log("_param :"+_param);
        try {
            if (_param != null) {
                var query = JSON.stringify(_param);
                //console.log("query :"+query);
                request.send(query);
            } else {
                request.send();
            }
            if (request.readyState == 4 && request.status == 200) {
                var response = request.responseText;
                if (_callback != null) {
                    _callback.call(this, response);
                } else {
                    return response;
                }
            }
        } catch (e) {
            return "";
        }
    }
};