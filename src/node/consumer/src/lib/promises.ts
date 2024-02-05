/**
 * Execute promises in sequence one after another.
 */
export async function sequence(promises: Array<() => Promise<any>>): Promise<any[]> {
    return promises.reduce((promise: Promise<any[]>, func: () => Promise<any>) =>
        promise.then(result => func().then(res => result.concat(res))), Promise.resolve([]));
}

/**
 * Return first resolved promise as the result.
 */
export async function first(promises: Array<Promise<any>>): Promise<any> {
    return Promise.all(promises.map(p => {
        // If a request fails, count that as a resolution so it will keep
        // waiting for other possible successes. If a request succeeds,
        // treat it as a rejection so Promise.all immediately bails out.
        return p.then((val) => Promise.reject(val), (err) => Promise.resolve(err));
    })).then(
        // If '.all' resolved, we've just got an array of errors.
        (errors) => Promise.reject(errors),
        // If '.all' rejected, we've got the result we wanted.
        (val) => Promise.resolve(val)
    );
}

/**
 * Delay promise
 */
export async function delay(duration: number): Promise<void> {
    return new Promise((resolve) => setTimeout(() => resolve(), duration));
}

/**
 * Timeout promise after a set time in ms
 */
export async function timeout(timeoutMs: number, promise: Promise<any>, message = 'Timed out'): Promise<any> {
    return Promise.race([
        promise,
        new Promise(function (resolve, reject) {
            setTimeout(function () {
                reject(message);
            }, timeoutMs);
        })
    ]);
}

/**
 * Return most common value from given array.
 */
export function mostCommonValue(array: any[]): any {
    return array.sort((a, b) => array.filter(v => v === a).length - array.filter(v => v === b).length).pop();
}