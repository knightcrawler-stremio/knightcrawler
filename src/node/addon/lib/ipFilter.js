import fs from 'fs';
import path from 'path';
import requestIp from 'request-ip';
import ip from 'ip';

const filePath = path.join(process.cwd(), 'allowed_ips.json');

let ALLOWED_ADDRESSES = [];
let ALLOWED_SUBNETS = [];

if (fs.existsSync(filePath)) {
    const allowedAddresses = JSON.parse(fs.readFileSync(filePath, 'utf8'));

    allowedAddresses.forEach(address => {
        if (address.indexOf('/') === -1) {
            ALLOWED_ADDRESSES.push(address);
        } else {
            ALLOWED_SUBNETS.push(address);
        }
    });
}

const IpIsAllowed = function(ipAddress) {
    if (ALLOWED_ADDRESSES.indexOf(ipAddress) > -1) {
        return true;
    }

    for (let i = 0; i < ALLOWED_SUBNETS.length; i++) {
        if (ip.cidrSubnet(ALLOWED_SUBNETS[i]).contains(ipAddress)) {
            return true;
        }
    }
    return false;
};

export const ipFilter = function (req, res, next) {
    const ipAddress = requestIp.getClientIp(req);

    if (ALLOWED_ADDRESSES.length === 0 && ALLOWED_SUBNETS.length === 0) {
        return next();
    }

    if (IpIsAllowed(ipAddress)) {
        return next();
    } else {
        console.log(`IP ${ipAddress} is not allowed`);
        res.status(404).send(null);
    }
};