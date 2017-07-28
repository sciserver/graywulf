__serviceName__Service.prototype.__methodName__ = function (__parameterList__) {
    var __me = this;
    var __pathParts = __pathParts__;
    var __queryParts = __queryParts__;
    var __data = __bodyParameter__;
    var __url = this.__createUrl(__pathParts, __queryParts);
    var __request = null;
    if (__data) {
        __request = {
            contentType: "application/json",
            data: JSON.stringify(__data)
        }
    };
    this.__callService(__url, "__httpMethod__", __request,
        function (result, status, xhr) {
            if (on_success) on_success(result);
        },
        function (xhr, status, message) {
            if (on_error) {
                on_error(xhr, status, message);
            } else {
                __me.error(xhr, status, message);
            }
        });
};

