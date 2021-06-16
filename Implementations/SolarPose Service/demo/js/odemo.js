function setTimeFactor(n) {
    timeFactor = n;
}
function getGeoPose(name, lat, lon, alt) {
    if (requestsInProgress > 2) {
        return;
    }
    const now = Date.now();
    var xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            var responseObject = JSON.parse(this.responseText);
            if (name == 'London') {
                document.querySelector("#londonResponse").innerHTML = (name + ': response = ' + this.responseText);
                if (responseObject.angles.pitch < 0.0) {
                    document.querySelector("#londonDayNight").innerHTML = "Night";
                } else {
                    document.querySelector("#londonDayNight").innerHTML = "Day";
                }
                yawLondon = responseObject.angles.yaw;

            } else if (name == 'Oslo') {
                document.querySelector("#osloResponse").innerHTML = (name + ': response = ' + this.responseText);
                if (responseObject.angles.pitch < 0.0) {
                    document.querySelector("#osloDayNight").innerHTML = "Night";
                } else {
                    document.querySelector("#osloDayNight").innerHTML = "Day";
                }
                yawOslo = responseObject.angles.yaw;
            } else if (name == 'Google') {
                document.querySelector("#googleResponse").innerHTML = (name + ': response = ' + this.responseText);
                if (responseObject.angles.pitch < 0.0) {
                    document.querySelector("#googleDayNight").innerHTML = "Night";
                } else {
                    document.querySelector("#googleDayNight").innerHTML = "Day";
                }
                yawGoogle = responseObject.angles.yaw;
            } else if (name == 'Tokyo') {
                document.querySelector("#tokyoResponse").innerHTML = (name + ': response = ' + this.responseText);
                if (responseObject.angles.pitch < 0.0) {
                    document.querySelector("#tokyoDayNight").innerHTML = "Night";
                } else {
                    document.querySelector("#tokyoDayNight").innerHTML = "Day";
                }
                yawTokyo = responseObject.angles.yaw;
            } else if (name == 'Geneva') {
                document.querySelector("#genevaResponse").innerHTML = (name + ': response = ' + this.responseText);
                if (responseObject.angles.pitch < 0.0) {
                    document.querySelector("#genevaDayNight").innerHTML = "Night";
                } else {
                    document.querySelector("#genevaDayNight").innerHTML = "Day";
                }
                yawGeneva = responseObject.angles.yaw;
            }
            requestsInProgress--;
        }

    };
    xhttp.open("GET", "https://service.geopose.io/solar/solarpose/ypr?longitude=" +
        lon.toString() + "&latitude=" + lat.toString() + "&height=" + alt.toString() + "&unixTimeMs=" + simTime.toString());
    xhttp.send();
    requestsInProgress++;
}
var onProg = function (xhr) {

    if (xhr.lengthComputable) {

        var percentComplete = xhr.loaded / xhr.total * 100;
        console.log(Math.round(percentComplete, 2) + '% downloaded');

    }

};

var onErr = function (xhr) {};
var requestsInProgress = 0;
var container, stats;
var camera, scene, renderer;
var mouseX = 0, mouseY = 0;
var windowHalfX = window.innerWidth / 2;
var windowHalfY = window.innerHeight / 2;
var lat = 50.5;
var lon = -1.46;
var alt = 16.0;

var yawTokyo = 0;
var yawLondon = 0;
var yawGoogle = 0;
var yawGeneva = 0;
var yawOslo = 0;

var OsloDalek = null;
var GenevaDalek = null;
var GoogleDalek = null;
var TokyoDalek = null;
var LondonDalek = null;

var startTime = Date.now();
var simTime = startTime;
var timeFactor = 256;

var dLight = null;
var isReady = false;

init();
animate();
var NEAR = 10, FAR = 3000;

function init() {
    container = document.createElement('div');
    document.body.appendChild(container);

    camera = new THREE.PerspectiveCamera(45, window.innerWidth / window.innerHeight, 1, 500);
    camera.position.x = -140;
    camera.position.y = 200;
    camera.position.z = 940;

    // scene

    scene = new THREE.Scene();
    scene.background = new THREE.Color(0xccccff);
    scene.fog = new THREE.Fog(0x59472b, 100, FAR);

    var ambientLight = new THREE.AmbientLight(0xffffff, 0.25);
    scene.add(ambientLight);

    const color = 0xFFFFFF;
    const intensity = 1.0;

    light = new THREE.SpotLight(0xffffff, 1, 0, Math.PI / 2);
    light.position.set(0, 1500, 1000);
    light.target.position.set(0, 0, 0);

    light.castShadow = true;

    light.shadow = new THREE.LightShadow(new THREE.PerspectiveCamera(50, 1, 1200, 2500));
    light.shadow.bias = 0.0001;
    var SHADOW_MAP_WIDTH = 2048,
    SHADOW_MAP_HEIGHT = 1024;

    light.shadow.mapSize.width = SHADOW_MAP_WIDTH;
    light.shadow.mapSize.height = SHADOW_MAP_HEIGHT;

    dLight = light;

    scene.add(light);
    scene.add(camera);

    var onProgress = function (xhr) {

        if (xhr.lengthComputable) {

            var percentComplete = xhr.loaded / xhr.total * 100;
            console.log(Math.round(percentComplete, 2) + '% downloaded');

        }

    };

    var onError = function (xhr) {};

    THREE.Loader.Handlers.add(/\.dds$/i, new THREE.DDSLoader());

    new THREE.MTLLoader().setPath('models/').load('dalek.mtl', function (materials) {

        materials.preload();

        new THREE.OBJLoader()
        .setMaterials(materials)
        .setPath('models/')
        .load('dalek.obj', function (object) {
            object.scale.x = 4.0;
            object.scale.y = 4.0;
            object.scale.z = 4.0;
            object.castShadow = true;
            object.rotation.y = 0.0; //Math.PI / 2;

            LondonDalek = object;
            OsloDalek = LondonDalek.clone();
            TokyoDalek = LondonDalek.clone();
            GenevaDalek = LondonDalek.clone();
            GoogleDalek = LondonDalek.clone();
            var deltaX = -6.0;
            var deltaY = 0.0;
            var deltaZ = -5.0;

            LondonDalek.position.set(-119 + deltaX * 0.0, 191.8, 908 + deltaZ * 0.0);
            OsloDalek.position.set(-122 + deltaX * 1.0, 191.8, 908 + deltaZ * 0.0);
            TokyoDalek.position.set(-127 + deltaX * 2.0, 191.8, 910 + deltaZ * 2.0);
            GenevaDalek.position.set(-131 + deltaX * 3.0, 191.8, 920 + deltaZ * 3.0);
            GoogleDalek.position.set(-134 + deltaX * 4.0, 191.8, 910 + deltaZ * 1.0);

            scene.add(OsloDalek);
            scene.add(GoogleDalek);
            scene.add(LondonDalek);
            scene.add(TokyoDalek);
            scene.add(GenevaDalek);

            isReady = true;
        }, onProgress, onError);

    });
    var map = new THREE.TextureLoader().load('textures/grid.jpg');
    map.wrapS = map.wrapT = THREE.RepeatWrapping;
    map.anisotropy = 16;
    var material = new THREE.MeshPhongMaterial({
            map: map,
            side: THREE.DoubleSide
        });

    object = new THREE.Mesh(new THREE.PlaneBufferGeometry(100, 100, 8, 8), material);
    object.position.set(-135.0, 190.5, 915.0);
    object.rotation.x = 3.0 * Math.PI / 2;
    scene.add(object);

    renderer = new THREE.WebGLRenderer({
            antialias: true
        });
    renderer.setPixelRatio(window.devicePixelRatio);
    renderer.setSize(window.innerWidth, window.innerHeight);
    container.appendChild(renderer.domElement);

    document.addEventListener('mousemove', onDocumentMouseMove, false);

    window.addEventListener('resize', onWindowResize, false);

}

function onWindowResize() {

    windowHalfX = window.innerWidth / 2;
    windowHalfY = window.innerHeight / 2;

    camera.aspect = window.innerWidth / window.innerHeight;
    camera.updateProjectionMatrix();

    renderer.setSize(window.innerWidth, window.innerHeight);

}

function onDocumentMouseMove(event) {

    mouseX = (event.clientX - windowHalfX) / 2;
    mouseY = (event.clientY - windowHalfY) / 2;

}

function animate() {
    simTime += timeFactor;
    var index = Math.trunc((simTime / 233) % 5);
    //alert('index ' + index);
    switch (index) {
    case 0:
        getGeoPose('London', 51.516, -0.131, 10.0);
        break;
    case 1:
        getGeoPose('Oslo', 59.917, 10.761, 5.0);
        break;
    case 2:
        getGeoPose('Tokyo', 35.684, 139.767, 4.0);
        break;
    case 3:
        getGeoPose('Geneva', 46.212, 6.125, 123.0);
        break;
    case 4:
        getGeoPose('Google', 37.423, -122.084, 7.0);
    }

    requestAnimationFrame(animate);
    render();

}

function render() {
    if (LondonDalek != null) {
        LondonDalek.rotation.y = (Math.PI) - ((yawLondon / 180.0) * Math.PI);

    }
    if (OsloDalek != null) {
        OsloDalek.rotation.y = (Math.PI) - ((yawOslo / 180.0) * Math.PI);
        //alert(yawOslo);
    }
    if (TokyoDalek != null) {
        TokyoDalek.rotation.y = (Math.PI) - ((yawTokyo / 180.0) * Math.PI);
    }
    if (GenevaDalek != null) {
        GenevaDalek.rotation.y = (Math.PI) - ((yawGeneva / 180.0) * Math.PI);
    }
    if (GoogleDalek != null) {
        GoogleDalek.rotation.y = (Math.PI) - ((yawGoogle / 180.0) * Math.PI);
    }

    dLight.rotation.y = Math.PI - yawLondon / Math.PI / 2;
    camera.lookAt(scene.position);
    renderer.render(scene, camera);
}
