import { PromiseHelpers } from '@helpers/promises_helpers';

describe('PromiseHelpers', () => {
    beforeAll(() => {
        jest.useFakeTimers({timerLimit: 5000});
    });

    afterAll(() => {
        jest.useRealTimers();
    });
    
    describe('sequence', () => {
        it('should resolve promises in sequence', async () => {
            const promises = [() => Promise.resolve(1), () => Promise.resolve(2), () => Promise.resolve(3)];
            const result = await PromiseHelpers.sequence(promises);
            expect(result).toEqual([1, 2, 3]);
        });
    });

    describe('first', () => {
        it('should resolve the first fulfilled promise', async () => {
            const promises = [Promise.reject('error'), Promise.resolve('success'), Promise.resolve('success2')];
            const result = await PromiseHelpers.first(promises);
            expect(result).toBe('success');
        });
    });

    describe('delay', () => {
        it('should delay execution', async () => {
            const startTime = Date.now();
            const delayPromise = PromiseHelpers.delay(1000);
            jest.runAllTimers();
            await delayPromise;
            const endTime = Date.now();
            expect(endTime - startTime).toBeGreaterThanOrEqual(1000);
        }, 30000);
    });

    describe('timeout', () => {
        it('should reject promise after timeout', async () => {
            const promise = new Promise((resolve) => setTimeout(resolve, 2000));
            const timeoutPromise = PromiseHelpers.timeout(1000, promise);
            jest.advanceTimersByTime(1000);
            await expect(timeoutPromise).rejects.toBe('Timed out');
        }, 20000);
    });

    describe('mostCommonValue', () => {
        it('should return the most common value in an array', () => {
            const array = [1, 2, 2, 3, 3, 3];
            const result = PromiseHelpers.mostCommonValue(array);
            expect(result).toBe(3);
        });
    });
});