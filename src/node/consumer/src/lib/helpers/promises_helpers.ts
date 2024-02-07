export const PromiseHelpers = {

    sequence: async function (promises: (() => Promise<any>)[]) {
        return promises.reduce((promise, func) =>
            promise.then(result => func().then(res => result.concat(res))), Promise.resolve([]));
    },

    first: async function (promises) {
        return Promise.all(promises.map(p => {
            return p.then((val) => Promise.reject(val), (err) => Promise.resolve(err));
        })).then(
            (errors) => Promise.reject(errors),
            (val) => Promise.resolve(val)
        );
    },

    delay: async function (duration: number) {
        return new Promise<void>(resolve => setTimeout(() => resolve(), duration));
    },

    timeout: async function (timeoutMs: number, promise, message = 'Timed out') {
        return Promise.race([
            promise,
            new Promise(function (resolve, reject) {
                setTimeout(function () {
                    reject(message);
                }, timeoutMs);
            })
        ]);
    },

    mostCommonValue: function (array) {
        return array.sort((a, b) => array.filter(v => v === a).length - array.filter(v => v === b).length).pop();
    }
};