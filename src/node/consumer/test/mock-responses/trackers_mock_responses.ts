import {http, HttpResponse} from "msw";

export const trackerTestResponse = http.get('https://ngosang.github.io/trackerslist/trackers_all.txt', () => {
    return HttpResponse.text('http://tracker1.com\nhttp://tracker2.com')
});