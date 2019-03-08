function cloneCallProcessAction() {
    var parameters = {};
    parameters.entityName = Xrm.Page.data.entity.getEntityReference().entityType;
    parameters.entityId = Xrm.Page.data.entity.getId().replace("{", "").replace("}", "");

    var req = new XMLHttpRequest();
    req.open("POST", Xrm.Page.context.getClientUrl() + "/api/data/v8.2/new_CloneRegistro", true);
    req.setRequestHeader("OData-MaxVersion", "4.0");
    req.setRequestHeader("OData-Version", "4.0");
    req.setRequestHeader("Accept", "application/json");
    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    req.onreadystatechange = function () {
        if (this.readyState === 4) {
            req.onreadystatechange = null;
            if ((this.status === 204) || (this.status === 200)) {
                var reqcloneId = "";
                if (req.response != "") {
                    reqcloneId = JSON.parse(req.response).cloneId;
                    if (reqcloneId) {
                        Xrm.Page.ui.setFormNotification("Record has submitted for cloning.", "INFO", "GEN")
                        setTimeout(function () {
                            reloadPage(reqcloneId);
                        }, 3000);
                    }
                }
            } else {
                var error;
                try {
                    error = JSON.parse(req.response).error;
                } catch (e) {
                    error = new Error("Unexpected Error");
                }
            }
        }
    };
    req.send(JSON.stringify(parameters));
}

