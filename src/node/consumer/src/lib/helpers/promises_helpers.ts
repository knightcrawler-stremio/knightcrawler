/* eslint-disable @typescript-eslint/no-explicit-any */
export const PromiseHelpers = {

    sequence: async function (promises: (() => Promise<any>)[]): Promise<any> {
        return promises.reduce((promise, func) =>
            promise.then(result => func().then(res => result.concat(res))), Promise.resolve([]));
    },

    first: async function (promises: any): Promise<any> {
        return Promise.all(promises.map((p: any) => {
            return p.then((val: any) => Promise.reject(val), (err: any) => Promise.resolve(err));
        })).then(
            (errors) => Promise.reject(errors),
            (val) => Promise.resolve(val)
        );
    },

    delay: async function (duration: number): Promise<void> {
        return new Promise<void>(resolve => setTimeout(() => resolve(), duration));
    },

    timeout: async function (timeoutMs: number, promise: any, message = 'Timed out'): Promise<any> {
        return Promise.race([
            promise,
            new Promise(function (resolve, reject) {
                setTimeout(function () {
                    reject(message);
                }, timeoutMs);
            })
        ]);
    },

    mostCommonValue: function (array: any[]): any {
        return array.sort((a, b) => array.filter(v => v === a).length - array.filter(v => v === b).length).pop();
    }
};
/* eslint-enable @typescript-eslint/no-explicit-any */