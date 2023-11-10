import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate } from 'k6/metrics';

const MAX_VUS = `${__ENV.MAX_VUS}`;
const BASE_URL = `${__ENV.API_DOMAIN}`;

export let errorRate = new Rate('errors');

export let options = {
    stages: [
        { duration: '80s', target: MAX_VUS },
        { duration: '100s', target: MAX_VUS },
        { duration: '40s', target: 0 },
    ],
};

export default function () {

    const id = createForecast();
    sleep(1);

    if (id) {
        getForecast(id);
    }

    sleep(2);
}

function getForecast(id) {
    let response = http.get(`${BASE_URL}/weather-forecasts/${id}`);
    checkResponse(response);
}

function createForecast() {

    let id = '';

    let response = http.post(`${BASE_URL}/weather-forecasts?latitude=50&longitude=0.12`, null);
    checkResponse(response);
    if (response.status == 201) {
        id = response.json().id;
    }

    return id;
}

function checkResponse(httpResponse) {
    check(httpResponse, { 'status was Ok': (r) => r.status >= 200 && r.status < 300 });
    if (httpResponse.status >= 400) {
        errorRate.add(1);
    }
}
